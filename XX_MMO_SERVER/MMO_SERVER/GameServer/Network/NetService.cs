using Common;
using Google.Protobuf;
using Proto;
using Serilog;
using Summer;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Network
{
    /// <summary>
    /// 网络服务
    /// </summary>
    public class NetService
    {
        TcpServer tcpServer;
        public NetService()
        {
            tcpServer = new TcpServer("0.0.0.0", 32510);
            tcpServer.Connected += OnClientConnected;
            tcpServer.Disconnected += OnDisconnected;
        }


        public void Start()
        {
            tcpServer.Start();
            MessageRouter.Instance.Start(10);
        }

        private void OnClientConnected(Connection connection)
        {
            Log.Debug("有客户端接入");
           
        }

        private void OnDisconnected(Connection connection)
        {
            Log.Debug("连接断开");
        }
    }
}
