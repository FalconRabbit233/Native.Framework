using Native.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using xyz.presidium.dicebot.Code.CQEvents;
using xyz.presidium.dicebot.Code.Dicebot;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace Native.Core
{
    /// <summary>
    /// 酷Q应用主入口类
    /// </summary>
    public class CQMain
    {
        /// <summary>
        /// 在应用被加载时将调用此方法进行事件注册, 请在此方法里向 <see cref="IUnityContainer"/> 容器中注册需要使用的事件
        /// </summary>
        /// <param name="container">用于注册的 IOC 容器 </param>
        public static void Register(IUnityContainer unityContainer)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dicebot.sqlite");
            var conn = new SQLiteConnection(
                new SQLiteConnectionString(path, false)
            );

            unityContainer
                .RegisterInstance(conn)
                .RegisterType<IGroupMessage, MessageHandler>("群消息处理")
                .RegisterType<IDiscussMessage, MessageHandler>("讨论组消息处理")
                .RegisterType<IPrivateMessage, MessageHandler>("私聊消息处理")
                .RegisterType<IFriendAddRequest, FriendsHandler>("好友添加请求处理");
        }
    }
}
