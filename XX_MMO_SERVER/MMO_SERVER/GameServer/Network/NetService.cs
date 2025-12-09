using Summer.Network;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Summer;

namespace GameServer.Network
{
    /// <summary>
    /// 网络服务
    /// </summary>
    public class NetService
    {
        //网络监听器
        TcpSocketListener listener = null;
        public NetService() { }

        public void Init(int port)
        {
            listener = new TcpSocketListener("0.0.0.0", port);
            listener.SocketConnected += OnClientConnected;
        }

        public void Start()
        {
            listener.Start();
        }

        private void OnClientConnected(object? sender, Socket socket)
        {
            var connection = new Connection(socket);
            connection.OnDataReceive += OnDataRecevie;
            connection.OnDisconnected += OnDisconnected;
        }

        private void OnDisconnected(Connection sender)
        {
            Console.WriteLine("连接断开");
        }

        private void OnDataRecevie(Connection sender, byte[] data)
        {
            Package package = Package.Parser.ParseFrom(data);
            //Package package = ProtobufTool.Parse<Package>(data);
            MessageRouter.Instance.AddMessage(sender, package);
        }
    }
}
