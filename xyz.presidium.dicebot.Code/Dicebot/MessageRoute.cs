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

            RegisterRoute<RollDiceController>(s =>
                new Regex(@"^.[rR][hH]?").IsMatch(s)
            );

            RegisterRoute<CointossController>(s =>
                new Regex(@"^.[fF]").IsMatch(s)
            );

            RegisterRoute<NicknameController>(s =>
                new Regex(@"^.[nN][nN]?").IsMatch(s)
            );

            RegisterRoute<GroupSwitchController>(s =>
                new Regex(@"^.switch").IsMatch(s)
            );

            RegisterRoute<DefaultDiceController>(s =>
                new Regex(@"^.[dD]").IsMatch(s)
            );
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
