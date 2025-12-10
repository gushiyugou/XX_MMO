using Google.Protobuf;
using Proto;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Summer
{
    public class MyConnection : Connection
    {
        public MyConnection(Socket socket) : base(socket)
        {
            package = _package as Package;
        }

        #region 发送网络数据包

        #region 网络数据包属性
        private Package package;
        public Request Request
        {
            get
            {

                if (package == null)
                {
                    package = new Package();
                }
                if (package.Request == null)
                {
                    package.Request = new Request();
                }
                return package.Request;
            }
        }
        public Response Response
        {
            get
            {

                if (package == null)
                {
                    package = new Package();
                }
                if (package.Response == null)
                {
                    package.Response = new Response();
                }
                return package.Response;
            }
        }
        #endregion


        #region 消息发送
        public override void Send()
        {
            if (_package != null) Send(_package);
            _package = null;
        }

        public override void Send(IMessage messagePackage)
        {
            byte[] data = null;
            //将messagePackage写入到字节流当中，转换为byte数组
            using (MemoryStream ms = new MemoryStream())
            {

               (messagePackage as Package).WriteTo(ms);
                //编码
                data = new byte[4 + ms.Length];
                Buffer.BlockCopy(BitConverter.GetBytes(ms.Length), 0, data, 0, 4);
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 4, (int)ms.Length);
            }
            Send(data, 0, data.Length);
        }

        public override void Send(byte[] data, int offset, int count)
        {
            lock (this)
            {
                if (socket.Connected)
                {
                    socket.BeginSend(data, offset, count, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
            }
        }

        protected override void SendCallback(IAsyncResult ar)
        {
            int len = socket.EndSend(ar);
        }

        #endregion


        #endregion

    }
}
