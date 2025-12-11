
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Proto;
using Serilog;
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
        public delegate void DataReceiveCallback(Connection sender,IMessage data);
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

            //创建解码器  协议调整
            var lfd = new SocketReceiver(socket);
            lfd.DataReceived += Received;
            lfd.Disconnected += () => OnDisconnected?.Invoke(this);
            lfd.Start();
        }

        private void Received(byte[] data)
        {
            //前提是data必须是大端数据
            ushort code = GetUShort(data,0);
            var message = ProtoTool.ParseFrom(code, data, 2, data.Length - 2);
            OnDataReceive?.Invoke(this, message);

            //using (var ds = DataStream.Allocate(data))
            //{
            //    ushort code = ds.ReadUShort();
            //    var message = ProtoTool.ParseFrom(code, data, 2, data.Length - 2);
            //    OnDataReceive?.Invoke(this, message);
            //}
        }




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
        #endregion


        #region 消息发送


        public void Send(IMessage message)
        {
            using (var ds = DataStream.Allocate())
            {
                int code = ProtoTool.SeqCode(message.GetType());
                ds.WriteInt(message.CalculateSize() + 2);
                ds.WriteUShort((ushort)code);
                message.WriteTo(ds);
                this.SocketSend(ds.ToArray());
            }

        }
        private void SocketSend(byte[] data)
        {
            this.SocketSend(data, 0, data.Length);
        }
        private void SocketSend(byte[] data,int offset,int len)
        {
            lock (this)
            {

                if (socket.Connected)
                {
                    socket.BeginSend(data, offset, len, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
            }
        }

        //前提是data必须是大端字节序
        private ushort GetUShort(byte[] data, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort)((data[offset] << 8) | data[offset + 1]);
            }
            else
            {
                return (ushort)((data[offset + 1] << 8) | data[offset]);
            }
        }

        protected void SendCallback(IAsyncResult ar)
        {
            int len = socket.EndSend(ar);
        }

        #endregion


        #endregion



        #region 弃用功能

        //private void Received(byte[] data)
        //{
        //    var package = Proto.Package.Parser.ParseFrom(data);
        //    var message = ProtoTool.Unpack(package);
        //    OnDataReceive?.Invoke(this, message);
        //}


        //public void Send(IMessage message)
        //{
        //    Proto.Package package = ProtoTool.Pack(message);
        //    byte[] data = null;
        //    //将messagePackage写入到字节流当中，转换为byte数组
        //    using (MemoryStream buf = new MemoryStream())
        //    {

        //        package.WriteTo(buf);
        //        int len = (int)buf.Length;

        //        //对消息进行编码
        //        data = new byte[4 + len];
        //        byte[] lenBytes = BitConverter.GetBytes(len);

        //        //BitConverter.IsLittleEndian为true时是小端序，为false时是大端序
        //        //为ture时，需要翻转数组转换为大端序的（网络序），在接收时再转成小端序
        //        if (!BitConverter.IsLittleEndian) Array.Reverse(lenBytes);
        //        //数据的拼装
        //        Array.Copy(lenBytes, 0, data, 0, 4);
        //        Array.Copy(buf.GetBuffer(), 0, data, 4, len);
        //    }
        //    Send(data, 0, data.Length);

        //}


        //private void Send(byte[] data, int offset, int len)
        //{
        //    lock (this)
        //    {

        //        if (socket.Connected)
        //        {
        //            Log.Debug("发送消息：len={0}", data.Length);
        //            //对消息进行编码
        //            byte[] buffer = new byte[4 + len];
        //            byte[] lenBytes = BitConverter.GetBytes(len);
        //            //BitConverter.IsLittleEndian为true时是小端序，为false时是大端序
        //            //为ture时，需要翻转数组转换为大端序的（网络序），在接收时再转成小端序
        //            if (!BitConverter.IsLittleEndian) Array.Reverse(lenBytes);
        //            //数据的拼装
        //            Array.Copy(lenBytes, 0, buffer, 0, 4);
        //            Array.Copy(data, offset, buffer, 4, len);
        //            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        //        }
        //    }
        //}



        #endregion
    }
}
