
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
        public delegate void DataReceiveCallback(NetConnection sender,byte[] data);
        public delegate void DisconnectedCallback(NetConnection sender);


        public Socket socket;
        private DataReceiveCallback dataReceiveCallback;
        private DisconnectedCallback disconnectedCallback;

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
    }
}
