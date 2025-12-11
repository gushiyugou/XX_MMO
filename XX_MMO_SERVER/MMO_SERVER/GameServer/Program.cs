using Common;
using GameServer.Network;
using Proto;
using Serilog;
using Summer.Network;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region 初始化日志系统
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() //debug info warn error
                .WriteTo.Console()
                .WriteTo.File("logs\\server-log.text", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            #endregion




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
            //Log.Information("发现用户登录请求:用户名={0},用户密码={1}", message.Username, message.Password);
        }
    }
}

