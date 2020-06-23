using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;
using System.Text.RegularExpressions;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    class DefaultDiceController : IResponsable
    {
        private SQLiteConnection context;

        public DefaultDiceController(SQLiteConnection context)
        {
            this.context = context;
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            var pattern = new Regex(@"[dD]\W*(\d+)");
            var commandResult = pattern.Match(message.Text);

            if (!commandResult.Success) return null;

            var diceExists = Utils.GetDefaultDice(context, fromQQ, fromGroup, fromDiscuss, out var defaultDice);

            var diceAfter = int.Parse(commandResult.Groups[1].Value);
            var diceBefore = defaultDice.DefaultDiceValue;

            defaultDice.DefaultDiceValue = diceAfter;

            if (diceExists)
                context.Update(defaultDice);
            else
                context.Insert(defaultDice);

            Utils.GetNickname(context, fromQQ, fromGroup, fromDiscuss, out var nickname);

            return $" * {nickname.NicknameValue} 变更默认骰子：{diceBefore} -> {diceAfter}";
        }
    }
}
