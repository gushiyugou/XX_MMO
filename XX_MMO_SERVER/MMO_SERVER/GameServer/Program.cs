using GameServer.Network;
using Network;
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



            Console.ReadKey();
        }

        
    }
}

