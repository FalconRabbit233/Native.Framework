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
    public class MessageHandler : IPrivateMessage, IGroupMessage, IDiscussMessage
    {
        private BotCore bot;

        public MessageHandler(BotCore bot)
        {
            this.bot = bot;
        }

        public void DiscussMessage(object sender, CQDiscussMessageEventArgs e)
        {
            e.Handler = bot.RecieveMessage(e.FromQQ, e.Message, e.FromDiscuss.SendDiscussMessage, null, e.FromDiscuss);
        }

        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            e.Handler = bot.RecieveMessage(e.FromQQ, e.Message, e.FromGroup.SendGroupMessage, e.FromGroup);
        }

        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            e.Handler = bot.RecieveMessage(e.FromQQ, e.Message, e.FromQQ.SendPrivateMessage);
        }
    }
}
