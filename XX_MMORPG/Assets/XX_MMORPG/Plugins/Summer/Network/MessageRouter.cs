using Summer;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Summer.Network
{

    #region 消息单元
    /// <summary>
    /// 消息单元
    /// </summary>
    public class Msg
    {
        public NetConnection sender;
        public IMessage message;

        public Msg(NetConnection sender, IMessage message)
        {
            this.sender = sender;
            this.message = message;
        }
    }

    #endregion

    /// <summary>
    /// 消息转发器
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {
        #region 属性相关
        int ThreadCount = 1;
        int WorkerCount = 0;
        bool Running = false;
        AutoResetEvent threadEvent = new AutoResetEvent(true);//通过set每次可以唤醒一个线程

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
        private Queue<Msg> messageQueue = new Queue<Msg>();

        #endregion

        #region 操作相关
        /// <summary>
        /// 消息入队操作
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="message">消息体</param>
        public void AddMessage(NetConnection sender, IMessage message)
        {
            messageQueue.Enqueue(new Msg(sender, message));
            threadEvent.Set();
        }

        #region 消息响应相关

        //订阅(subscribing)
        public void Subscribing<T>(MessageHandler<T> handler) where T :IMessage
        {
            string key = typeof(T).FullName;
            if (!delegateMap.ContainsKey(key))
            {
                delegateMap[key] = null;
            }
            delegateMap[key] = (MessageHandler<T>)delegateMap[key] + handler;
        }

        //消息触发
        private void EventTrigger<T>(NetConnection sender,T message)
        {
            string type = typeof(T).FullName;
            if (delegateMap.ContainsKey(type))
            {
                MessageHandler<T> handler = (MessageHandler<T>)delegateMap[type];
                try
                {
                    handler?.Invoke(sender, message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"MessageRouter.Fire error:{ex.StackTrace}");
                }
                
            }
        }
        //退订unsubscribe
        public void Unsubscribing<T>(MessageHandler<T> handler)
        {
            string key = typeof(T).FullName;
            if (delegateMap.ContainsKey(key))
            {
                delegateMap[key] = null;
            }
            delegateMap[key] = (MessageHandler<T>)delegateMap[key] - handler;
        }

        #endregion

        #region 服务器状态相关
        public void Start(int ThreadCount)
        {
            Running = true;
            this.ThreadCount = Math.Clamp(ThreadCount, 1, 201);
            for(int i = 0; i< this.ThreadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageWork));
            }
            while (WorkerCount < this.ThreadCount)
            {
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            Running = false;
            messageQueue.Clear();
            while(WorkerCount > 0)
            {
                threadEvent.Set();
            }
            Thread.Sleep(100);
        }
        #endregion

        #region 消息处理逻辑
        private void MessageWork(object? state)
        {

            Console.WriteLine("Worker Thread Start");
            try
            {
                WorkerCount = Interlocked.Increment(ref WorkerCount);
                while (Running)
                {
                    if (messageQueue.Count == 0) 
                    {
                        //暂停在当前位置，不再往下执行
                        threadEvent.WaitOne();
                        continue;
                    }
                    Msg msg = messageQueue.Dequeue();
                    IMessage package = msg.message;

                    if(package != null)
                    {
                        executeMessage(msg.sender,package);
                    }
                }
            }
            catch 
            { }
            finally
            {
                WorkerCount = Interlocked.Increment(ref WorkerCount);
            }
            Console.WriteLine("Worker Thread End");
        }


        private void executeMessage(NetConnection sender ,IMessage message)
        {
            //发送消息，马上触发订阅
            var eventTriggerMethod = this.GetType().GetMethod("EventTrigger",
                BindingFlags.NonPublic | BindingFlags.Instance);
            //调用泛型方法
            var met = eventTriggerMethod.MakeGenericMethod(message.GetType());
            met.Invoke(this, new object[] { sender, message });


            var type = message.GetType();
            foreach (var t in type.GetProperties())
            {
                if (t.Name == "Parser" || t.Name == "Descriptor") continue;
                var value = t.GetValue(message);
                if (value != null)
                {
                    if (typeof(IMessage).IsAssignableFrom(value.GetType()))
                    {
                        //Console.WriteLine("发现消息，触发订阅，继续递归");

                        executeMessage(sender, (IMessage)value);
                    }
                }
            }
        }





            #region 弃用
        /// <summary>
        /// 根据反射原理对消息进行自动分发 ,已弃用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        //private void Execute(NetConnection sender,object message)
        //{
        //    var eventTriggerMethod = this.GetType().GetMethod("EventTrigger",
        //        BindingFlags.NonPublic | BindingFlags.Instance);
        //    Type type = message.GetType();

        //    foreach (var t in type.GetProperties())
        //    {
        //        if (t.Name == "Parser" || t.Name == "Descriptor") continue;
        //        var value = t.GetValue(message);
        //        if (value != null)
        //        {
        //            //调用泛型方法
        //            var met = eventTriggerMethod.MakeGenericMethod(value.GetType());
        //            met.Invoke(this, new object[] { sender, value });
        //        }
        //    }
        //}
            #endregion


        #endregion

        #endregion
    }
}
