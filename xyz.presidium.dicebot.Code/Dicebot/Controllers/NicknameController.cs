using Native.Sdk.Cqp;
using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Models;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    public class NicknameController : IResponsable
    {
        private static SQLiteConnection context;

        public NicknameController(SQLiteConnection context)
        {
            NicknameController.context = context;
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            var renamePattern = new Regex(@"([nN][nN]?)\W*(.{0,16})");
            var commandResult = renamePattern.Match(message.Text);

            if (!commandResult.Success) return null;

            string replySuffix;
            Func<long, long> modifyGroup;

            switch (commandResult.Groups[1].Length)
            {
                case 2:
                    replySuffix = "在此处的";
                    modifyGroup = g => fromGroup.Id;
                    break;
                default:
                case 1:
                    replySuffix = "的全局";
                    modifyGroup = g => 0;
                    break;
            }

            var nicknameAfter = commandResult.Groups[2].Value;

            var nicknameExists = Utils.GetNickName(context, fromQQ, fromGroup, fromDiscuss, out var nickname);
            var nicknameBefore = nickname.NicknameValue;
            nickname.NicknameValue = nicknameAfter;

            nickname.FromGroup = modifyGroup(nickname.FromGroup);

            Func<object, int> updateMethod = nicknameExists ?
                (Func<object, int>)context.Update :
                (Func<object, int>)context.Insert;

            updateMethod(nickname);

            var reply = $" * {nicknameBefore} {replySuffix}新名字是 {nicknameAfter}";
            return reply;
        }
    }
}
