

using Google.Protobuf;
using Proto;
using System.Net;
using System.Net.Sockets;




//服务器IP、端口号
var host = "127.0.0.1";
int port = 32510;

//服务器终端
IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);
Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
socket.Connect(ipe);

Console.WriteLine("成功连接到服务器");


Vector3 v = new Vector3() { X=100, Y=100, Z=100 };
SendMessage(socket, v.ToByteArray());
Console.ReadLine();
// 添加 Vector3 到字节数组的扩展方法


// 修改调用处


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