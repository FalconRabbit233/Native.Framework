using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;
using xyz.presidium.dicebot.Code.Dicebot.Models;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    class ActionController : IResponsable
    {

        private SQLiteConnection context;

        public ActionController(SQLiteConnection context)
        {
            this.context = context;
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            var pattern = new Regex(@"[aA]\W*(\d+)[:：]{2}(.+)");
            var commandResult = pattern.Match(message.Text);

            if (!commandResult.Success) return null;

            if (!long.TryParse(commandResult.Groups[1].Value, out var tGroupId)) return null;

            if (context.Table<GroupWhitelist>().Where(g => g.enabledGroup == tGroupId).Count() < 1)
                return null;

            Utils.GetNickname(context, fromQQ, new CqpModel.Group(fromQQ.CQApi, tGroupId), fromDiscuss, out var nickname);

            var textToSend = $" * {nickname.NicknameValue} {commandResult.Groups[2].Value}";

            var sendMsg = fromQQ.CQApi.SendGroupMessage(tGroupId, textToSend);

            return sendMsg.IsSuccess ? $"{tGroupId}: {textToSend}" : "转发失败";
        }
    }
}
