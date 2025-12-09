
using Google.Protobuf;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    /// <summary>
    /// 客户端网络连接对象
    /// 职责：发送消息，接收消息回调，关闭连接，断开通知回调
    /// </summary>
    public class NetConnection
    {
        #region 网络连接属性
        public delegate void DataReceiveCallback(NetConnection sender,byte[] data);
        public delegate void DisconnectedCallback(NetConnection sender);


        public Socket socket;
        private DataReceiveCallback dataReceiveCallback;
        private DisconnectedCallback disconnectedCallback;

        #endregion


        #region 连接相关
        public NetConnection(Socket socket,DataReceiveCallback cb1,DisconnectedCallback cb2)
        {
            this.socket = socket;
            dataReceiveCallback = cb1;
            disconnectedCallback = cb2;

            //创建解码器
            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.dataReceivedHandler += OnDataRectived;
            lfd.disconnectedHandler += (socket) => disconnectedCallback?.Invoke(this);
            lfd.Start();
        }
  

        private void OnDataRectived(byte[] buffer)
        {
            //反序列化(字节转对象)
            dataReceiveCallback?.Invoke(this,buffer);
        }

        /// <summary>
        /// 主动关闭连接
        /// </summary>
        public void Close()
        {
            try { socket.Shutdown(SocketShutdown.Both); } catch { }
            socket.Close();
            socket = null;
            disconnectedCallback?.Invoke(this);
        }

        #endregion



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
                if(_package.Request == null)
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
            if(_package != null) Send(_package);
            _package = null;
        }

        public void Send(Package messagePackage)
        {
            byte[] data = null;
            //将messagePackage写入到字节流当中，转换为byte数组
            using(MemoryStream ms = new MemoryStream())
            {

                messagePackage.WriteTo(ms);
                //编码
                data = new byte[4 + ms.Length];
                Buffer.BlockCopy(BitConverter.GetBytes(ms.Length), 0, data, 0, 4);
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 4, (int)ms.Length);
            }
            Send(data,0,data.Length);
        }

        public void Send(byte[] data,int offset,int count)
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
