using Google.Protobuf;
using Proto;
using Summer.Network;
using System;
using System.Collections.Generic;
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
        }

        #region 发送网络数据包

        #region 网络数据包属性
        private Package _package;
        public Request Request
        {
            get
            {

                if (_package == null)
                {
                    _package = new Package();
                }
                if (_package.Request == null)
                {
                    _package.Request = new Request();
                }
                return _package.Request;
            }
        }
        public Response Response
        {
            get
            {

                if (_package == null)
                {
                    _package = new Package();
                }
                if (_package.Response == null)
                {
                    _package.Response = new Response();
                }
                return _package.Response;
            }
        }
        #endregion


        #region 消息发送
        public void Send()
        {
            if (_package != null) Send(_package);
            _package = null;
        }

        public void Send(Package messagePackage)
        {
            byte[] data = null;
            //将messagePackage写入到字节流当中，转换为byte数组
            using (MemoryStream ms = new MemoryStream())
            {

                messagePackage.WriteTo(ms);
                //编码
                data = new byte[4 + ms.Length];
                Buffer.BlockCopy(BitConverter.GetBytes(ms.Length), 0, data, 0, 4);
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 4, (int)ms.Length);
            }
            Send(data, 0, data.Length);
        }

        public void Send(byte[] data, int offset, int count)
        {
            lock (this)
            {
                if (socket.Connected)
                {
                    socket.BeginSend(data, offset, count, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            int len = socket.EndSend(ar);
        }

        #endregion


        #endregion

    }
}
