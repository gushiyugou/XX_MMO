

using Google.Protobuf;
using System.Net;
using System.Net.Sockets;
using System.Numerics;




//服务器IP、端口号
var host = "127.0.0.1";
int port = 32510;

//服务器终端
IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);
Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
socket.Connect(ipe);

Console.WriteLine("成功连接到服务器");


Proto.Vector3 vector = new Proto.Vector3() { X=100, Y=100, Z=100 };

//用户登录消息
Proto.Package package = new Proto.Package();
package.Request =new Proto.Request();
package.Request.UserLogin = new Proto.UserLoginRequest();
package.Request.UserLogin.Username = "XX";
package.Request.UserLogin.Password = "12345";

MemoryStream rawOutPut = new MemoryStream();
CodedOutputStream output = new CodedOutputStream(rawOutPut);
package.WriteTo(output);
output.Flush();
SendMessage(socket, package.ToByteArray());


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