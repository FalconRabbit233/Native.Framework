using Native.Sdk.Cqp;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    public interface IResponsable
    {
        string Response(QQ fromQQ, QQMessage message, Group fromGroup, Discuss fromDiscuss);
    }
}
