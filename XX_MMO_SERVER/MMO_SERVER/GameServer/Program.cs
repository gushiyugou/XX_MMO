using Summer.Network;
using GameServer.Network;
using Proto;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            int port = 32510;
            NetService netService = new NetService();
            netService.Start();


            MessageRouter.Instance.Start(4);

            MessageRouter.Instance.Subscribing<UserLoginRequest>(OnUserLoginRequest);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private static void OnUserLoginRequest(Connection sender, UserLoginRequest message)
        {
            Console.WriteLine($"发现用户登录请求:用户名={message.Username},用户密码={message.Password}");
        }
    }
}

