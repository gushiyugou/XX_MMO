using Google.Protobuf;
using Proto;
using Summer;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NetClient
{
   
    private static Connection connection = null;
    public static void ConnectToServer(string host,int port)
    {
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(host), port);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ipe);//通过服务器IP和端口建立连接
        Debug.Log("客户端连接成功");
        connection = new Connection(socket);
        connection.OnDataReceive += OnReceive;
        connection.OnDisconnected += OnDisconnected;
        MessageRouter.Instance.Start(4);
    }


    public static void Send(Proto.Package package)
    {
        if(connection != null)
        {
            connection.Send(package);
        }
    }

    //连接成功
    private static void OnReceive(Connection sender, byte[] data)
    {
        
        //发送消息
        Package message = Package.Parser.ParseFrom(data);
        MessageRouter.Instance.AddMessage(sender, message);
    }

    //断开连接
    private static void OnDisconnected(Connection sender)
    {
        Debug.Log("断开连接");
    }

    #region 发送消息请求逻辑

    public static void SendRequest(IMessage message)
    {
        var package = new Proto.Package() { Request = new Proto.Request() };
        foreach (var p in package.Request.GetType().GetProperties())
        {
            if (p.Name == "Parser" || p.Name == "Descriptor") continue;
            if (p.PropertyType == message.GetType())
            {
                p.SetValue(package.Request, message);
            }
        }
        connection.Send(package);
    }
    #endregion
}
