using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static GameServer.Network.NetConnection;

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
            new NetConnection(socket,
                new NetConnection.DataReceiveCallback(OnDataRecevie),
                new NetConnection.DisconnectedCallback(OnDisconnected));
        }

        private void OnDisconnected(NetConnection sender)
        {
            Console.WriteLine("连接断开");
        }

        private void OnDataRecevie(NetConnection sender, byte[] data)
        {
            Vector3 v = Vector3.Parser.ParseFrom(data);
            Console.WriteLine(v.ToString());
        }
    }
}
