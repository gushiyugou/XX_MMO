using Google.Protobuf;
using Google.Protobuf.Reflection;
using Serilog;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Summer
{
    /// <summary>
    /// Protobuf序列化与反序列化
    /// </summary>
    public class ProtoTool
    {
        /// <summary>
        /// 序列化protobuf
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] Serialize(IMessage msg)
        {
            using (MemoryStream rawOutput = new MemoryStream())
            {
                msg.WriteTo(rawOutput);
                byte[] result = rawOutput.ToArray();
                return result;
            }
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public static T Parse<T>(byte[] dataBytes) where T : IMessage, new()
        {
            T msg = new T();
            msg = (T)msg.Descriptor.Parser.ParseFrom(dataBytes);
            return msg;
        }




        private static Dictionary<string, Type> _registry = new Dictionary<string, Type>();
        private static Dictionary<int,Type> mDict1 = new Dictionary<int,Type>();
        private static Dictionary<Type,int> mDict2 = new Dictionary<Type, int>();

        static ProtoTool()
        {
            List<string> list = new List<string>();
            var q = from t in Assembly.GetExecutingAssembly().GetTypes() select t;
            q.ToList().ForEach(t =>
            {
                if (typeof(IMessage).IsAssignableFrom(t))
                {
                    var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
                    _registry.Add(desc.FullName, t);
                    list.Add(desc.FullName);
                }
            });


            list.Sort((x, y) =>{
                //按照名字字符长度排序
                if(x.Length != y.Length) return x.Length - y.Length;
                //如果长度相同
                return string.Compare(x, y, StringComparison.Ordinal);
            });

            for(int i = 0; i < list.Count; i++)
            {
                var fname = list[i];
                var type = _registry[fname];
                Log.Information("Proto类型注册，{0} - {1}",i, fname);
                mDict1[i] = type;
                mDict2[type] = i;
            }
        }

        public static int SeqCode(Type type)
        {
            return mDict2[type];
        }
        public static Type SeqType(int code)
        {
            return mDict1[code];
        }

        public static IMessage ParseFrom(int typeCode, byte[] data, int offset, int length)
        {
            Type type = SeqType(typeCode);
            var desc = type.GetProperty("Descriptor").GetValue(type) as MessageDescriptor;
            var message = desc.Parser.ParseFrom(data, offset, length);
            Log.Information("解析消息：code={0} - {1}", typeCode, message);
            return message;
        }



        #region 弃用

        public static Proto.Package Pack(IMessage message)
        {
            Proto.Package package = new Proto.Package();
            package.FullName = message.Descriptor.FullName;
            package.Data = message.ToByteString();
            return package;
        }

        public static IMessage Unpack(Proto.Package package)
        {
            string fullName = package.FullName;
            if (_registry.ContainsKey(fullName))
            {
                Type msgType = _registry[fullName];
                var desc = msgType.GetProperty("Descriptor").GetValue(msgType) as MessageDescriptor;
                return desc.Parser.ParseFrom(package.Data);
            }
            Log.Warning("不存在{0}类型的消息", fullName);
            return null;
        }


        
        //public static IMessage ParseFrom(int typeCode, byte[] data, int offset, int len)
        //{
        //    Type t = ProtoTool.SeqType(typeCode);
        //    var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
        //    var msg = desc.Parser.ParseFrom(data, offset, len);
        //    Log.Information("解析消息：code={0} - {1}", typeCode, msg);
        //    return msg;
        //}


        #endregion

    }
}
