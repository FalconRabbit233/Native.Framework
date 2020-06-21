using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using xyz.presidium.dicebot.Code.Dicebot;

namespace xyz.presidium.dicebot.Code.CQEvents
{
    public class FriendsHandler : IFriendAddRequest
    {
        private BotCore bot;
        public FriendsHandler(BotCore botCore)
        {
            bot = botCore;
        }
        public void FriendAddRequest(object sender, CQFriendAddRequestEventArgs e)
        {
            var handleResult = bot.HandleInvite(e.FromQQ.Id, e.AppendMessage);
            switch (handleResult)
            {
                case BotCore.InviteResult.Approved:
                    e.Request.SetFriendAddRequest(
                        Native.Sdk.Cqp.Enum.CQResponseType.PASS,
                        "Approved"
                        );
                    break;

                case BotCore.InviteResult.Denied:
                    e.Request.SetFriendAddRequest(
                        Native.Sdk.Cqp.Enum.CQResponseType.FAIL,
                        "Denied"
                        );
                    break;

                default:
                case BotCore.InviteResult.Ignored:
                    break;
            }
            e.Handler = true;
        }
    }
}
