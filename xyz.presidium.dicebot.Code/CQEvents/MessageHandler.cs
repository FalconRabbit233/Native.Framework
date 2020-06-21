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
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            e.FromQQ.SendPrivateMessage(e.Message);
            e.Handler = true;
        }
    }
}
