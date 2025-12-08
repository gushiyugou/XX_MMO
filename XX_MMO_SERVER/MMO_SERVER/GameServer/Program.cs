using Common.Network;
using GameServer.Network;
using Proto;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 32510;
            NetService netService = new NetService();
            netService.Init(port);
            netService.Start();


            MessageRouter.Instance.Start(4);

            MessageRouter.Instance.Subscribing<UserLoginRequest>(OnUserLoginRequest);
            Console.ReadKey();
        }

        private static void OnUserLoginRequest(NetConnection sender, UserLoginRequest message)
        {
            Console.WriteLine($"发现用户登录请求:用户名={message.Username},用户密码={message.Password}");
        }
    }
}

