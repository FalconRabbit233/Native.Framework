using System;
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

            RegisterRoute<RollDiceController>(s => Regex.Match(s, @"^.[rR][hH]?[^p]").Success);

            RegisterRoute<JrrpController>(s => Regex.Match(s, @"^.[jJ][rR]{2}[pP]").Success);

            RegisterRoute<RppkController>(s => Regex.Match(s, @"^.[rR][pP]{2}[kK]").Success);

            RegisterRoute<RpArenaController>(s => Regex.Match(s, @"^.[rR][pP][aA]").Success);

            RegisterRoute<Coc7Controller>(s => Regex.Match(s, @"^.[cC][oO][cC]7").Success);

            RegisterRoute<CointossController>(s => Regex.Match(s, @"^.[fF]").Success);

            RegisterRoute<ActionController>(s => Regex.Match(s, @"^.[aA]").Success);

            RegisterRoute<RumorController>(s => Regex.Match(s, @"^.[uU]").Success);

            RegisterRoute<NicknameController>(s => Regex.Match(s, @"^.[nN][nN]?").Success);

            RegisterRoute<DefaultDiceController>(s => Regex.Match(s, @"^.[dD]").Success);

            RegisterRoute<HelpController>(s => Regex.Match(s, @"^.[hH]").Success);

            RegisterRoute<GroupSwitchController>(s => Regex.Match(s, @"^.switch").Success);
        }

        private void RegisterRoute<T>(Predicate<string> predicate) where T : IResponsable
        {
            routes.Add(new Tuple<Predicate<string>, Type>(predicate, typeof(T)));
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
