using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;
using System.Text.RegularExpressions;
using Unity.Interception.Utilities;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    class RollDiceController : IResponsable
    {
        private SQLiteConnection context;

        private static Regex compiledOverallPattern;

        private static Regex compiledDiceCommandPattern;

        private Rng rng;

        public RollDiceController(SQLiteConnection context, Rng rng)
        {
            this.context = context;
            this.rng = rng;
        }

        private int minusOp(int i) => -i;

        private int plusOp(int i) => i;

        private Func<int, int> SignToOperation(string sign)
        {
            switch (sign)
            {
                case "-":
                    return minusOp;

                case "+":
                default:
                    return plusOp;
            }
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            if (compiledOverallPattern == null)
            {
                // group 1: h, 2: (x)#, 3: 3d6+1d3+5+d+2d+d8, 4: 注释
                compiledOverallPattern
                    = new Regex(
                        @"[rR]([hH]?)\s*(?:(\d+)#)?((?:[+-]?(?:(?:\d*d\d*)|(?:\d+)))+)\s*(.+?)?$",
                        RegexOptions.Compiled);
            }

            var overallResult = compiledOverallPattern.Match(message.Text);

            if (!overallResult.Success) return null;

            if (!int.TryParse(overallResult.Groups[2].Value, out var repeatCount))
                repeatCount = 1;

            if (compiledDiceCommandPattern == null)
                // 1: [+-null], 2: (x)d6, 3: 2d(x), 4: 整数
                compiledDiceCommandPattern
                    = new Regex(@"([+-]?)(?:(?:(\d*)d(\d*))|(\d+))", RegexOptions.Compiled);

            var diceCommands = compiledDiceCommandPattern.Matches(overallResult.Groups[3].Value);

            Utils.GetDefaultDice(context, fromQQ, fromGroup, fromDiscuss, out var defaultDiceUpper);
            Utils.GetNickname(context, fromQQ, fromGroup, fromDiscuss, out var nickname);

            var diceCommandRecovery = "";
            var sumDiceCount = 0;
            foreach (Match c in diceCommands)
            {
                diceCommandRecovery += c.Groups[1].Value;
                if (c.Groups[4].Value != "")
                {
                    diceCommandRecovery += c.Groups[4].Value;
                }
                else
                {
                    var diceCount = int.TryParse(c.Groups[2].Value, out var validDiceCount) ?
                        validDiceCount : 1;

                    sumDiceCount += diceCount * repeatCount;
                    if (sumDiceCount > 128) return $" * {nickname.NicknameValue} 扔了一大堆骰子，都不知道滚哪儿去了";

                    var diceDim = int.TryParse(c.Groups[3].Value, out var validDiceDim) ?
                        validDiceDim : defaultDiceUpper.DefaultDiceValue;

                    if (diceDim > 256) return $" * {nickname.NicknameValue} 扔了一个球";

                    diceCommandRecovery += $"{diceCount}d{diceDim}";
                }
            }

            var jrrp = Utils.GetJrrp(fromQQ, fromGroup, fromDiscuss);
            var repeatResults = new List<List<List<int>>>();
            for (int j = 0; j < repeatCount; j++)  // 3#层面的展开
            {
                var commandResults = new List<List<int>>();
                foreach (Match diceCommand in diceCommands)  // 3d6+d8层面的展开
                {
                    var commandResult = new List<int>();

                    var commandSign = diceCommand.Groups[1].Value ?? "+";

                    if (diceCommand.Groups[4].Value != "")  // 数字加减
                    {
                        var algoCommand = int.Parse(diceCommand.Groups[4].Value);

                        commandResult.Add(SignToOperation(commandSign)(algoCommand));
                    }
                    else  // 骰子加减
                    {
                        if (!int.TryParse(diceCommand.Groups[2].Value, out var diceCount))
                            diceCount = 1;

                        if (!int.TryParse(diceCommand.Groups[3].Value, out var diceUpper))
                            diceUpper = defaultDiceUpper.DefaultDiceValue;

                        for (int i = 0; i < diceCount; i++)  // 3d6层面的展开
                        {
                            var rollResult = rng.RollWithJrrp(diceUpper, jrrp);
                            commandResult.Add(SignToOperation(commandSign)(rollResult));
                        }
                    }
                    commandResults.Add(commandResult);
                }
                repeatResults.Add(commandResults);
            }

            string rollCommandResult;

            if (repeatResults.Count == 1)
            {
                var commandsResult = repeatResults[0];

                var containsMultiDice = commandsResult
                    .Aggregate(false, (csAcc, cs) => csAcc || cs.Count > 1);

                if (commandsResult.Count == 1)
                {
                    var commandResult = commandsResult[0];

                    if (containsMultiDice)
                        rollCommandResult = $"{commandResult.JoinStrings(", ")} = {commandResult.Sum()}";
                    else
                        rollCommandResult = $"{commandResult.Sum()}";
                }
                else
                {
                    var displayItems = commandsResult
                        .Select(cs => new
                        {
                            textDisplay = cs.Count > 1 ?
                                $"+({cs.JoinStrings(",")})" :
                                cs[0] > 0 ? $"+{cs[0]}" : cs[0].ToString(),
                            numberSum = cs.Sum(),
                            sumDisplay = cs.Sum() > 0 ? $"+{cs.Sum()}" : cs.Sum().ToString(),
                        })
                        .ToList();

                    if (displayItems[0].textDisplay[0] == '+')
                        displayItems[0] = new
                        {
                            textDisplay = displayItems[0].textDisplay.Substring(1),
                            numberSum = displayItems[0].numberSum,
                            sumDisplay = displayItems[0].sumDisplay.Substring(1),
                        };

                    var section1 = displayItems.JoinStrings("", i => i.textDisplay);
                    var section2 = displayItems.JoinStrings("", i => i.sumDisplay);
                    var section3 = displayItems.Sum(i => i.numberSum);

                    if (containsMultiDice)
                        rollCommandResult = $"{section1} = {section2} = {section3}";
                    else
                        rollCommandResult = $"{section1} = {section3}";
                }
            }
            else
            {
                var repeatPlain = repeatResults.Select(  // 3#展开
                    rs => rs.Aggregate(0,  // 3d6+d8展开
                        (csAcc, cs) => csAcc + cs.Aggregate(0,  // 3d6展开
                            (cAcc, c) => cAcc + c
                        )
                    )
                 );

                rollCommandResult = $"{{ {repeatPlain.JoinStrings(", ")} }}";
            }

            var repeatDisplay = overallResult.Groups[2].Value == "" ? "" : $"{repeatCount}次";

            var totalResult = $" * {nickname.NicknameValue} 投掷 {overallResult.Groups[4].Value}: {repeatDisplay}{diceCommandRecovery} = {rollCommandResult}";

            if (overallResult.Groups[1].Value != "")
            {
                fromQQ.SendPrivateMessage(totalResult);
                return $" * {nickname.NicknameValue} 投了一把魔法骰子，你们这些笨蛋是看不见的";
            }
            else
            {
                return totalResult;
            }
        }
    }
}
