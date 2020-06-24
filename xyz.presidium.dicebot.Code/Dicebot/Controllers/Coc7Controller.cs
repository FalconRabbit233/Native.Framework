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
    class Coc7Controller : IResponsable
    {
        private SQLiteConnection context;

        private Rng rng;

        public Coc7Controller(SQLiteConnection context, Rng rng)
        {
            this.context = context;
            this.rng = rng;
        }


        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            var pattern = new Regex(@"[cC][oO][cC]7\s*(\d?)");
            var commandResult = pattern.Match(message.Text);

            if (!commandResult.Success) return null;

            var jrrp = Utils.GetJrrp(fromQQ, fromGroup, fromDiscuss);

            var generateCount = int.TryParse(commandResult.Groups[1].Value, out var countInput) ?
                countInput > 5 ? 5 : countInput :
                1;

            int d6(int x)
            {
                var sum = 0;
                for (int i = 0; i < x; i++)
                {
                    sum += rng.RollWithJrrp(6, jrrp);
                }
                return sum;
            }

            int normalAttrGen() => d6(3) * 5;
            int stableAttrGen() => (d6(2) + 6) * 5;

            var singleCardModel = new Tuple<string, Func<int>>[9]
            {
                new Tuple<string, Func<int>>("力", normalAttrGen),
                new Tuple<string, Func<int>>("体", normalAttrGen),
                new Tuple<string, Func<int>>("型", stableAttrGen),
                new Tuple<string, Func<int>>("敏", normalAttrGen),
                new Tuple<string, Func<int>>("貌", normalAttrGen),
                new Tuple<string, Func<int>>("智", stableAttrGen),
                new Tuple<string, Func<int>>("意", normalAttrGen),
                new Tuple<string, Func<int>>("教", stableAttrGen),
                new Tuple<string, Func<int>>("运", normalAttrGen),
            };

            var ressultOutput = new List<string>();
            for (int i = 0; i < generateCount; i++)
            {
                var card = singleCardModel
                    .Select(attr => new { name = attr.Item1, value = attr.Item2() })
                    .ToArray();

                var cardSum = card.Sum(c => c.value);

                var cardText = card.JoinStrings(" ", c => $"{c.name}{c.value}");
                ressultOutput.Add($"{cardText} 和{cardSum}");
            }

            Utils.GetNickname(context, fromQQ, fromGroup, fromDiscuss, out var nickname);

            return $" * {nickname.NicknameValue} 生成 COC7 人物卡:\n{ressultOutput.JoinStrings("\n")}";
        }
    }
}
