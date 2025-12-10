
using Google.Protobuf;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Network
{
    /// <summary>
    /// 客户端网络连接对象
    /// 职责：发送消息，接收消息回调，关闭连接，断开通知回调
    /// </summary>
    public class Connection
    {
        #region 网络连接属性
        public delegate void DataReceiveCallback(Connection sender,byte[] data);
        public delegate void DisconnectedCallback(Connection sender);

        private Socket _socket;
        public Socket socket { get { return _socket; } }
        public DataReceiveCallback OnDataReceive;
        public DisconnectedCallback OnDisconnected;

        #endregion


        #region 连接相关
        public Connection(Socket socket)
        {
            _socket = socket;

            //创建解码器
            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.DataReceived += Received;
            lfd.Disconnected += () => OnDisconnected?.Invoke(this);
            lfd.Start();
        }

        private void Received(byte[] data)
        {
            OnDataReceive?.Invoke(this, data);
        }

        


        //private void Received(byte[] buffer)
        //{
        //    //反序列化(字节转对象)
        //    OnDataReceive?.Invoke(this,buffer);
        //}

        /// <summary>
        /// 主动关闭连接
        /// </summary>
        public void Close()
        {
            try { socket.Shutdown(SocketShutdown.Both); } catch { }
            socket.Close();
            _socket = null;
            OnDisconnected?.Invoke(this);
        }

        #endregion



        #region 发送网络数据包

        #region 网络数据包属性
        protected IMessage _package;
        #endregion


        #region 消息发送
        public void Send()
        {
            if (_package != null) Send(_package);
            _package = null;
        }



        public void Send(IMessage message)
        {
            byte[] data = null;
            //将messagePackage写入到字节流当中，转换为byte数组
            using(MemoryStream ms = new MemoryStream())
            {
                int len = (int)ms.Length;
                message.WriteTo(ms);
                //对消息进行编码
                data = new byte[4 + len];
                byte[] lenBytes = BitConverter.GetBytes(len);
                if(!BitConverter.IsLittleEndian) Array.Reverse(lenBytes);

                //数据的拼装
                Array.Copy(lenBytes, 0, data, 0, 4);
                Array.Copy(ms.GetBuffer(), 0, data, 4, len);
                //Buffer.BlockCopy(BitConverter.GetBytes(ms.Length), 0, data, 0, 4);
                //Buffer.BlockCopy(ms.GetBuffer(), 0, data, 4, len);
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

        protected void SendCallback(IAsyncResult ar)
        {
            int len = socket.EndSend(ar);
        }

            #endregion


        #endregion
    }
}
