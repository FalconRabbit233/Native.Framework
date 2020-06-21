using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyz.presidium.dicebot.Code.Dicebot
{
    public partial class BotCore
    {
        public InviteResult HandleInvite(long fromQQ, string msg)
        {
            if (msg == "2333")
            {
                return InviteResult.Approved;
            }
            else
            {
                return InviteResult.Denied;
            }
        }

        public enum InviteResult
        {
            Approved,
            Denied,
            Ignored
        }
    }
}
