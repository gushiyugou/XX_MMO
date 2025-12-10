using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Network
{
    /// <summary>
    /// 负责监听TCP网络端口，异步接收Socket连接
    /// --Connected         有新的连接
    /// --DataReceived      有新的消息请求
    /// --Disconnected      有连接断开
    /// --Start()           启动服务器
    /// --Stop()            关闭服务器
    /// --IsRunning         是否正在运行
    /// </summary>
    public class TcpServer
    {
        #region 属性相关
        private IPEndPoint endPoint;
        private Socket serverSocket;    //服务端监听对象
        private int backlog = 100;

        /// <summary>
        /// 委托
        /// </summary>
        /// <param name="connection"></param>
        public delegate void ConnectedCallback(Connection connection);
        public delegate void DataReceivedCallback(Connection connection, byte[] data);
        public delegate void DisconnectedCallback(Connection connection);


        /// <summary>
        /// 基于委托的事件
        /// Connected 事件委托：新的连接
        /// DataReceived 事件委托：收到消息
        /// Disconnected 事件委托：连接断开
        /// </summary>
        public event ConnectedCallback Connected;
        public event DataReceivedCallback DataReceived;
        public event DisconnectedCallback Disconnected;


        public event EventHandler<Socket> SocketConnected; //客户端接入事件
        #endregion
        public TcpServer(string host, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public TcpServer(string host, int port,int backlog):this(host,port)
        {
            this.backlog = backlog;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(backlog);
                Console.WriteLine("开始监听端口：" + endPoint.Port);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnAccept; //当有人连入的时候
                serverSocket.AcceptAsync(args);
            }
            else
            {
                Console.WriteLine("TcpServer is already running");
            }
        }

        private void OnAccept(object? sender, SocketAsyncEventArgs e)
        {
            Socket client = e.AcceptSocket; //连入的客户端
            //继续接收下一位
            e.AcceptSocket = null;
            serverSocket.AcceptAsync(e);
            //真的有人连进来
            if (e.SocketError == SocketError.Success)
            {
                
                if (client!=null)
                {
                    OnUpdateConnection(client);
                }
                
            }
        }

        /// <summary>
        ///新的socket接入
        /// </summary>
        /// <param name="client"></param>
        private void OnUpdateConnection(Socket client)
        {
            SocketConnected?.Invoke(this, client);
            Connection newConnection = new Connection(client);
            newConnection.OnDataReceive += (conn, data) => DataReceived?.Invoke(conn, data);
            newConnection.OnDisconnected += (conn) => Disconnected?.Invoke(conn);
            Connected?.Invoke(newConnection);
        }

        public bool IsRunning
        {
            get { return serverSocket != null; }
        }

        public void Stop()
        {
            if (serverSocket == null) return;
            serverSocket.Close();
            serverSocket = null;
        }

    }
}
