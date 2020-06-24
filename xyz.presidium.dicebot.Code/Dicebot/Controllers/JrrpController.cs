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
    class JrrpController : IResponsable
    {
        private SQLiteConnection context;

        public JrrpController(SQLiteConnection context)
        {
            this.context = context;
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            Utils.GetNickname(context, fromQQ, fromGroup, fromDiscuss, out var nickname);

            var jrrp = Utils.GetJrrp(fromQQ, fromGroup, fromDiscuss);
            return $" * {nickname.NicknameValue} 今天在此处的人品值是 {101 - jrrp}%";
        }
    }
}
