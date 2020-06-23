using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;

namespace xyz.presidium.dicebot.Code.CQEvents
{
    public class MessageHandler : IPrivateMessage, IGroupMessage
    {
        private BotCore bot;

        public MessageHandler(BotCore bot)
        {
            this.bot = bot;
        }

        public void DiscussMessage(object sender, CQDiscussMessageEventArgs e)
        {
            bot.RecieveMessage(e.FromQQ, e.Message, e.FromDiscuss.SendDiscussMessage, null, e.FromDiscuss);
            e.Handler = true;
        }

        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            bot.RecieveMessage(e.FromQQ, e.Message, e.FromGroup.SendGroupMessage, e.FromGroup);
            e.Handler = true;
        }

        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            bot.RecieveMessage(e.FromQQ, e.Message, e.FromQQ.SendPrivateMessage);
            e.Handler = true;
        }
    }
}
