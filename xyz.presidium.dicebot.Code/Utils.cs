﻿using Native.Sdk.Cqp;
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

        public static bool GetNickName(
            SQLiteConnection context,
            QQ fromQQ,
            Group fromGroup,
            Discuss fromDiscuss,
            out Nickname nickname)
        {
            var valid_group = GetValidGroup(fromGroup, fromDiscuss);

            Func<Nickname, bool> predicate = n => n.FromGroup == valid_group && n.FromQQ == fromQQ.Id;
            Func<Nickname, bool> globalNamePredicate = n => n.FromGroup == 0 && n.FromQQ == fromQQ.Id;

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
    }
}
