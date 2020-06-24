using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    class RppkController : IResponsable
    {
        private SQLiteConnection context;

        public RppkController(SQLiteConnection context)
        {
            this.context = context;
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
            return $" * {cNickname.NicknameValue}({cJrrp}%) 挑战 {tNickname.NicknameValue}({tJrrp}%)：{jrrpReview}({jrrpResult})";
        }
    }
}
