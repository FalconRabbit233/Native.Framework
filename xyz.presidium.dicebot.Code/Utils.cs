using Native.Sdk.Cqp;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Models;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code
{
    class Utils
    {
        /// <summary>
        /// 获取可用的群号
        /// </summary>
        /// <param name="group">群对象</param>
        /// <param name="discuss">讨论组对象</param>
        /// <returns>群号或讨论组号或0</returns>
        public static long GetValidGroup(Group group, Discuss discuss)
        {
            return group == null ?
                discuss ?? 0L :
                group.Id;
        }

        public static bool GetNickname(
            SQLiteConnection context,
            QQ fromQQ,
            Group fromGroup,
            Discuss fromDiscuss,
            out Nickname nickname)
        {
            var valid_group = GetValidGroup(fromGroup, fromDiscuss);

            bool predicate(Nickname n) => n.FromGroup == valid_group && n.FromQQ == fromQQ.Id;
            bool globalNamePredicate(Nickname n) => n.FromGroup == 0 && n.FromQQ == fromQQ.Id;

            var nameExists = context.Table<Nickname>().Count(predicate) > 0;

            nickname = nameExists ?
                context.Table<Nickname>().First(predicate) :
                context.Table<Nickname>().Count(globalNamePredicate) > 0 ?
                    context.Table<Nickname>().First(globalNamePredicate) :
                    new Nickname()
                    {
                        FromGroup = valid_group,
                        FromQQ = fromQQ.Id,
                        NicknameValue = valid_group == 0L ? "你" : CQApi.CQCode_At(fromQQ).ToSendString()
                    };

            return nameExists;
        }

        public static bool GetDefaultDice(
            SQLiteConnection context,
            QQ fromQQ,
            Group fromGroup,
            Discuss fromDiscuss,
            out DefaultDice defaultDice)
        {
            var valid_group = GetValidGroup(fromGroup, fromDiscuss);

            bool predicate(DefaultDice n) => n.FromGroup == valid_group && n.FromQQ == fromQQ.Id;

            var diceExists = context.Table<DefaultDice>().Count(predicate) > 0;

            defaultDice = diceExists ?
                context.Table<DefaultDice>().First(predicate) :
                new DefaultDice()
                {
                    FromGroup = valid_group,
                    FromQQ = fromQQ.Id,
                    DefaultDiceValue = 100
                };

            return diceExists;
        }

        /// <summary>
        /// 返回今日人品值，1好100坏
        /// </summary>
        /// <param name="fromQQ"></param>
        /// <param name="fromGroup"></param>
        /// <param name="fromDiscuss"></param>
        /// <returns>1-100</returns>
        public static int GetJrrp(QQ fromQQ, Group fromGroup, Discuss fromDiscuss)
        {
            var valid_group = GetValidGroup(fromGroup, fromDiscuss);

            var today = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));

            var seed = today.Date.ToString("yyyyMMdd") + fromQQ.Id.ToString() + valid_group.ToString();

            var rng = new Random(seed.GetHashCode());

            return rng.Next(1, 101);
        }
    }
}
