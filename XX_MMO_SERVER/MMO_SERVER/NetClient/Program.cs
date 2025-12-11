

using Summer.Network;
using Google.Protobuf;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using Summer;
using Common;
using Serilog;



#region 初始化日志系统
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() //debug info warn error
    .WriteTo.Console()
    .WriteTo.File("logs\\client-log.text", rollingInterval: RollingInterval.Day)
    .CreateLogger();

#endregion

//服务器IP、端口号
var host = "127.0.0.1";
int port = 32510;

//服务器终端
IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);
Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
socket.Connect(ipe);//通过服务器IP和端口建立连接
Log.Debug("成功连接到服务器");
Thread.Sleep(200);








//用户登录消息

Connection conn = new Connection(socket);
var msg = new Proto.UserLoginRequest();
msg.Username = "阿斯顿";
msg.Password = "123456";
conn.Send(msg);
conn.Send(msg);
conn.Send(msg);
conn.Send(msg);
Proto.Vector3 v = new Proto.Vector3() { X = 3, Y = 4, Z = 5 };
conn.Send(v);
conn.Send(v);
conn.Send(v);

//var pack = ProtoHelper.Pack(msg);
//var res = ProtoHelper.Unpack(pack);
//Log.Information("{0} : {1}",res.GetType(),res);

//long ts = 557594037927940;
//byte[] arr = DataSerializer.VarintEncode2(ts);
//Log.Information("{0} - {1}", ts, arr.Length);



object[] prs = new object[] {
    sbyte.MinValue,
    sbyte.MaxValue,
    byte.MinValue,
    byte.MaxValue,
    short.MinValue,
    short.MaxValue,
    ushort.MinValue,
    ushort.MaxValue,
    int.MinValue,
    int.MaxValue,
    uint.MinValue,
    uint.MaxValue,
    long.MaxValue,
    long.MinValue,
    ulong.MaxValue,
    ulong.MinValue,
    (byte)127,
    (sbyte)-100,
    (short)1024,
    (short)-100,
    65535,
    int.MaxValue,
    uint.MaxValue,
    8080L,
    3.14f,
    //new Summer.Vec3i(){X=3,Y=4,Z=5},
    //new Summer.Vec3i(){X=10,Y=20},
    1.414,
    false,
    "毛主席万岁",
    34234234.532d,
    "落霞与孤鹜齐飞",
    "秋水共长天一色"
};

byte[] rrr = DataSerializer.Serialize(prs);
object[] rr2 = DataSerializer.Deserialize(rrr);
foreach (var o in rr2)
{
    Log.Information("{0}", o);
}
Log.Information("Data序列化长度={0}", rrr.Length);



Log.Information("执行结束");

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



//void SendRequest(IMessage message)
//{
//    var package = new Proto.Package() { Request = new Proto.Request() };
//    foreach (var p in package.Request.GetType().GetProperties())
//    {
//        if (p.Name == "Parser" || p.Name == "Descriptor") continue;
//        if(p.PropertyType == message.GetType())
//        {
//            p.SetValue(package.Request, message);
//        }
//    }
//    connection.Send(package);
//}
#endregion