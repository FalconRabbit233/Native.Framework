using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;
using xyz.presidium.dicebot.Code.Dicebot.Models;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    class GroupSwitchController : IResponsable
    {
        private SQLiteConnection context;

        public GroupSwitchController(SQLiteConnection context)
        {
            this.context = context;
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            var pattern = new Regex(@"switch\W*(\d+)");
            var commandResult = pattern.Match(message.Text);

            if (!commandResult.Success) return null;

            if (context.Table<SuperAdmin>().Count(s => s.QQNumber == fromQQ.Id) < 1) return null;

            if (!long.TryParse(commandResult.Groups[1].Value, out var operatingGroup)) return null;

            bool whereGroup(GroupWhitelist g) => g.enabledGroup == operatingGroup;

            var group_enabled = context.Table<GroupWhitelist>()
                .Count(whereGroup) > 0;

            string operatingResult;
            if (group_enabled)
            {
                context.Delete(context.Table<GroupWhitelist>().First(whereGroup));
                operatingResult = "停";
            }
            else
            {
                context.Insert(new GroupWhitelist() { enabledGroup = operatingGroup });
                operatingResult = "启";
            }

            return $" * 群 {operatingGroup} 现已{operatingResult}用";
        }
    }
}
