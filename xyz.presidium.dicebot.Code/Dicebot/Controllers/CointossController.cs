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
    public class CointossController : IResponsable
    {
        private SQLiteConnection context;

        private static Random rng;

        public CointossController(SQLiteConnection context)
        {
            this.context = context;

            if (rng == null) rng = new Random();
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            var pattern = new Regex(@"[fF]\W*(.*)");
            var commandResult = pattern.Match(message.Text);

            if (!commandResult.Success) return null;

            //获取事件
            var Event = commandResult.Groups[1].Value;

            //获取当前用户
            Utils.GetNickname(context, fromQQ, fromGroup, fromDiscuss, out var nickname);
            var nicknameCurrent = nickname.NicknameValue;

            //获取硬币结果
            var coinSide = rng.Next(2) == 0 ? "反面" : "正面";

            //输出字符串
            return $" * {nicknameCurrent} 投掷硬币 {Event}：{coinSide}";
        }
    }
}
