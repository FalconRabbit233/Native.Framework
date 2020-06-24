﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Controllers;

namespace xyz.presidium.dicebot.Code.Dicebot
{
    public partial class BotCore
    {
        private void RouteInit()
        {
            if (routes.Count != 0) return;

            RegisterRoute<RollDiceController>(s => new Regex(@"^.[rR][hH]?[^p]").IsMatch(s));

            RegisterRoute<JrrpController>(s => new Regex(@"^.[jJ][rR]{2}[pP]").IsMatch(s));

            RegisterRoute<RppkController>(s => new Regex(@"^.[rR][pP]{2}[kK]").IsMatch(s));

            RegisterRoute<Coc7Controller>(s => new Regex(@"^.[cC][oO][cC]7").IsMatch(s));

            RegisterRoute<CointossController>(s => new Regex(@"^.[fF]").IsMatch(s));

            RegisterRoute<NicknameController>(s => new Regex(@"^.[nN][nN]?").IsMatch(s));

            RegisterRoute<DefaultDiceController>(s => new Regex(@"^.[dD]").IsMatch(s));

            RegisterRoute<GroupSwitchController>(s => new Regex(@"^.switch").IsMatch(s));
        }

        private BotCore RegisterRoute<T>(Predicate<string> predicate) where T : IResponsable
        {
            routes.Add(new Tuple<Predicate<string>, Type>(predicate, typeof(T)));
            return this;
        }

        private Type Route(string message)
        {
            foreach (var tuple in routes)
            {
                if (tuple.Item1(message))
                {
                    return tuple.Item2;
                }
            }

            return null;
        }
    }
}
