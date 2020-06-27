using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;
using Native.Sdk.Cqp;
using xyz.presidium.dicebot.Code.Dicebot.Models;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    class RppkController : IResponsable
    {
        private SQLiteConnection context;

        private Rng rng;

        public RppkController(SQLiteConnection context, Rng rng)
        {
            this.context = context;
            this.rng = rng;
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            var cqAts = message.CQCodes
                 .Where(c => c.Function == Native.Sdk.Cqp.Enum.CQFunction.At)
                 .ToList();

            QQ chanllenger;
            QQ target;

            switch (cqAts.Count)
            {
                case 1:
                    chanllenger = fromQQ;
                    if (long.TryParse(cqAts[0].Items["qq"], out var qq))
                        target = new QQ(fromQQ.CQApi, qq);
                    else return null;
                    break;

                case 2:
                    if (long.TryParse(cqAts[0].Items["qq"], out var cqq))
                        chanllenger = new QQ(fromQQ.CQApi, cqq);
                    else return null;

                    if (long.TryParse(cqAts[1].Items["qq"], out var tqq))
                        target = new QQ(fromQQ.CQApi, tqq);
                    else return null;
                    break;

                default:
                    return null;
            }

            var cJrrp = 101 - Utils.GetJrrp(chanllenger, fromGroup, fromDiscuss);
            var tJrrp = 101 - Utils.GetJrrp(target, fromGroup, fromDiscuss);
            var jrrpResult = cJrrp - tJrrp;

            string jrrpReview;
            if (jrrpResult > 50) jrrpReview = "史诗大捷";
            else if (jrrpResult > 25) jrrpReview = "酣畅大胜";
            else if (jrrpResult > 10) jrrpReview = "略处上风";
            else if (jrrpResult > 5) jrrpReview = "血战险胜";
            else if (jrrpResult > -5) jrrpReview = "平分秋色";
            else if (jrrpResult > -10) jrrpReview = "血战惜败";
            else if (jrrpResult > -25) jrrpReview = "略处下风";
            else jrrpReview = "悲惨失败";


            Utils.GetNickname(context, chanllenger, fromGroup, fromDiscuss, out var cNickname);
            Utils.GetNickname(context, target, fromGroup, fromDiscuss, out var tNickname);

            var resultText = $" * {cNickname.NicknameValue}({cJrrp}%) 挑战 {tNickname.NicknameValue}({tJrrp}%)：{jrrpReview}({jrrpResult})";

            var validGroup = Utils.GetValidGroup(fromGroup, fromDiscuss);
            var selfMemberType = fromQQ.CQApi.GetGroupMemberInfo(validGroup, fromQQ.CQApi.GetLoginQQ().Id).MemberType;
            var targetMemberType = fromQQ.CQApi.GetGroupMemberInfo(validGroup, target.Id).MemberType;

            if (validGroup != 0L && fromGroup != null &&
                (int)selfMemberType > (int)targetMemberType &&
                jrrpResult > 0)
            {
                fromQQ.CQApi.SendGroupMessage(validGroup, resultText);

                bool wherePrisoner(PrisonTime p) => p.FromGroup == validGroup && p.FromQQ == target.Id;
                PrisonTime prisonTime;

                // 初始化计数对象
                if (context.Table<PrisonTime>().Count(wherePrisoner) < 1)
                {
                    prisonTime = new PrisonTime() { FromGroup = validGroup, FromQQ = target.Id, PrisonTimeValue = 0 };
                    context.Insert(prisonTime);
                }
                else
                {
                    prisonTime = context.Table<PrisonTime>().First(wherePrisoner);
                }

                int banSecs = 7200 / (100 - jrrpResult);
                // 执行禁言
                var exeLines = context.Table<ExecutionLine>().ToArray();
                var exeText = string.Format(
                    exeLines[rng.Roll(exeLines.Length) - 1].Content,
                    cNickname.NicknameValue,
                    tNickname.NicknameValue,
                    banSecs,
                    prisonTime.PrisonTimeValue
                    ) + $" ({prisonTime.PrisonTimeValue}+{banSecs})";

                var exeTextResult = fromQQ.CQApi.SendGroupMessage(validGroup, exeText);

                if (fromQQ.CQApi.SetGroupMemberBanSpeak(validGroup, target, TimeSpan.FromSeconds(banSecs)))
                {
                    prisonTime.PrisonTimeValue += banSecs;
                    context.Update(prisonTime);
                }
                else if (exeTextResult.IsSuccess)
                {
                    fromQQ.CQApi.RemoveMessage(exeTextResult.Id);
                }

                return null;
            }
            else
            {
                return resultText;
            }

        }
    }
}
