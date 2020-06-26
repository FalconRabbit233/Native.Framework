using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;
using xyz.presidium.dicebot.Code.Dicebot.Controllers;
using xyz.presidium.dicebot.Code.Dicebot.Models;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot
{
    public partial class BotCore
    {
        public delegate QQMessage ReplyMethod(params object[] messsage);

        private SQLiteConnection context;

        private IUnityContainer container;

        public static List<Tuple<Predicate<string>, Type>> routes;

        public BotCore(SQLiteConnection context, IUnityContainer container)
        {
            routes = new List<Tuple<Predicate<string>, Type>>();
            this.container = container;
            this.context = context;
        }

        /// <summary>
        /// 文字指令总入口
        /// </summary>
        /// <param name="fromQQ">qq对象</param>
        /// <param name="message">消息对象</param>
        /// <param name="callback">回复函数</param>
        /// <param name="fromGroup">群对象</param>
        /// <param name="fromDiscusss">讨论组对象</param>
        public void RecieveMessage(
            QQ fromQQ,
            QQMessage message,
            ReplyMethod callback,
            CqpModel.Group fromGroup = null,
            Discuss fromDiscusss = null)
        {
            if (fromGroup != null &&
                context.Table<GroupWhitelist>()
                    .Count(g => g.enabledGroup == fromGroup.Id) < 1 &&
                context.Table<SuperAdmin>()
                    .Count(s => s.QQNumber == fromQQ.Id) < 1) return;

            if (message.Text[0] != '.' && message.Text[0] != '。') return;

            RouteInit();

            var controller = Route(message.Text);

            if (controller == null) return;

            var response = ((IResponsable)container.Resolve(controller))
                .Response(fromQQ, message, fromGroup, fromDiscusss);

            if (response == null) return;

            callback(response);
        }
    }
}
