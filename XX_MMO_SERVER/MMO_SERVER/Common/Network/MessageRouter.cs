using Common;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    /// <summary>
    /// 消息单元
    /// </summary>
    public class MsgUnit
    {
        public NetConnection sender;
        public IMessage message;

        public MsgUnit(NetConnection sender, IMessage message)
        {
            this.sender = sender;
            this.message = message;
        }
    }

    /// <summary>
    /// 消息转发器
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {
        /// <summary>
        /// 消息处理器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public delegate void MessageHandler<T>(NetConnection sender,T message);
        /// <summary>
        /// 消息类型（消息频道）
        /// </summary>
        private Dictionary<string, Delegate> delegateMap = new Dictionary<string, Delegate>();


        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<MsgUnit> messageQueue = new Queue<MsgUnit>();

        
        /// <summary>
        /// 消息入队操作
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="message">消息体</param>
        public void AddMessage(NetConnection sender, IMessage message)
        {
            messageQueue.Enqueue(new MsgUnit(sender, message));
            Console.WriteLine(messageQueue.Count);
        }

        //TODO:订阅(subscribing)
        public void Subscribing<T>(MessageHandler<T> handler) where T :IMessage
        {
            string key = typeof(T).Name;
            if (!delegateMap.ContainsKey(key))
            {
                delegateMap[key] = null;
            }
            delegateMap[key] = (MessageHandler<T>)delegateMap[key] + handler;
        }



        //TODO:退订unsubscribe
        public void Unsubscribing<T>(MessageHandler<T> handler)
        {
            string key = typeof(T).Name;
            if (delegateMap.ContainsKey(key))
            {
                delegateMap[key] = null;
            }
            delegateMap[key] = (MessageHandler<T>)delegateMap[key] - handler;
        }
    }
}
