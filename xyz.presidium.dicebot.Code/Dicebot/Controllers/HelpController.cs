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
using Unity.Interception.Utilities;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    public class HelpController : IResponsable
    {
        private SQLiteConnection context;

        public HelpController(SQLiteConnection context)
        {
            this.context = context;
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            var pattern = new Regex(@"[hH]([hH]?)");
            var commandResult = pattern.Match(message.Text);

            if (!commandResult.Success) return null;

            var helpInfos = new Dictionary<Type, string>()
            {
                { typeof(ActionController), "指令：.a {群号}::{文本}\n\t功能：向{群号}转发{文本}" },
                { typeof(Coc7Controller), "指令：.coc7 {数字: 最大5}\n\t功能：生成{数字}个CoC-7人物属性组" },
                { typeof(DefaultDiceController), "指令：.d {数字}\n\t功能：设置默认骰子为1d{数字}" },
                { typeof(CointossController), "指令：.f\n\t功能：投掷一枚硬币" },
                { typeof(HelpController), "指令：.h(h)\n\t功能：私聊(直接发)帮助说明" },
                { typeof(JrrpController), "指令：.jrrp\n\t功能：查看今日人品" },
                { typeof(NicknameController), "指令：.n(n) {昵称: 最大16字}\n\t功能：设置全局(本地)昵称为{昵称}" },
                { typeof(RollDiceController), "指令：.r(h) {骰子表达式: 1个以上}\n\t功能：掷骰子(暗投)\n\t{骰子表达式}：(z#)(+-)(x)d(y) 或 (+-)数字\n\t例：.rd，.r3d6，.rh3#d10+d6-3" },
                { typeof(RppkController), "指令：.rppk {@群友1} ({@群友2})\n\t功能：向{群友1}发起人品对战 (让{群友1}挑战{群友2}的人品)" },
                { typeof(RumorController), "指令：.u {群号}::{文本}\n\t功能：向{群号}匿名转发{文本}" }
            };

            var helpText = BotCore.routes
                .Select(r => helpInfos.TryGetValue(r.Item2, out var text) ? text : "")
                .JoinStrings("\n");

            helpText = $"基本规则：所有指令必须以{{点}}或{{句号}}开头，\n\t一般来说指令名不区分大小写\n\n{helpText}";

            if (commandResult.Groups[1].Value == "" && Utils.GetValidGroup(fromGroup, fromDiscuss) != 0L)
            {
                var msg = fromQQ.SendPrivateMessage(helpText);

                Utils.GetNickname(context, fromQQ, fromGroup, fromDiscuss, out var nickname);
                return msg.IsSuccess ?
                    $" * 已向 {nickname.NicknameValue} 单独投放使用说明书" :
                    $" * 向 {nickname.NicknameValue} 发送说明书失败, 请尝试.hh命令";
            }
            else return helpText;
        }
    }
}
