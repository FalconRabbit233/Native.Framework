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
            long modifiedGroup;
            bool recordExists;

            var nicknameAfter = commandResult.Groups[2].Value;

            var nicknameExists = Utils.GetNickname(context, fromQQ, fromGroup, fromDiscuss, out var nickname);
            var nicknameBefore = nickname.NicknameValue;
            nickname.NicknameValue = nicknameAfter;

            switch (commandResult.Groups[1].Length)
            {
                case 2:
                    replySuffix = "在此处的";
                    modifiedGroup = Utils.GetValidGroup(fromGroup, fromDiscuss);
                    recordExists = nicknameExists;
                    break;
                default:
                case 1:
                    replySuffix = "的全局";
                    modifiedGroup = 0;
                    recordExists = context.Table<Nickname>()
                        .Count(n => n.FromGroup == 0 && n.FromQQ == fromQQ.Id) > 0;
                    break;
            }

            nickname.FromGroup = modifiedGroup;

            if (recordExists)
                context.Update(nickname);
            else
                context.Insert(nickname);

            var reply = $" * {nicknameBefore} {replySuffix}新名字是 {nicknameAfter}";
            return reply;
        }
    }
}
