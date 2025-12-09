

using Summer.Network;
using Google.Protobuf;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using Summer;




//服务器IP、端口号
var host = "127.0.0.1";
int port = 32510;

//服务器终端
IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);
Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
socket.Connect(ipe);//通过服务器IP和端口建立连接
Console.WriteLine("成功连接到服务器");


Proto.Vector3 vector = new Proto.Vector3() { X=100, Y=100, Z=100 };
//NetConnection connection = new NetConnection(socket, null, null);
MyConnection connection = new MyConnection(socket);

Thread.Sleep(100);
//构建发送
Proto.Package package = new Proto.Package();
package.Request =new Proto.Request();
package.Request.UserLogin = new Proto.UserLoginRequest();
package.Request.UserLogin.Username = "XX";
package.Request.UserLogin.Password = "12345";
connection.Send(package);

//快捷发送
connection.Request.UserLogin = new Proto.UserLoginRequest();
connection.Request.UserLogin.Username = "WYX";
connection.Request.UserLogin.Password = "12456";
connection.Send();

Console.ReadLine();



static void SendMessage(Socket socket, byte[] body)
{
    byte[] lenBytes = BitConverter.GetBytes(body.Length);
    socket.Send(lenBytes);
    socket.Send(body);
}


#region 弃用功能
//Vector3序列化的常规做法
//static byte[] Vector3ToByteArray(Vector3 v)
//{
//    byte[] bytes = new byte[12];
//    Buffer.BlockCopy(BitConverter.GetBytes(v.X), 0, bytes, 0, 4);
//    Buffer.BlockCopy(BitConverter.GetBytes(v.Y), 0, bytes, 4, 4);
//    Buffer.BlockCopy(BitConverter.GetBytes(v.Z), 0, bytes, 8, 4);
//    return bytes;
//}
#endregion