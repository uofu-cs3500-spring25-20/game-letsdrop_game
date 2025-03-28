// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using CS3500.Networking;
using System.Text;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    // 保存所有客户端连接
    private static readonly List<NetworkConnection> clients = new();

    // 保存客户端连接对应的名字
    private static readonly Dictionary<NetworkConnection, string> clientNames = new();

    // 锁对象，防止多线程同时修改共享数据
    private static readonly object lockObj = new();

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main( string[] args )
    {
        Server.StartServer( HandleConnect, 11_000 );
        Console.Read(); // don't stop the program.
    }


    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect( NetworkConnection connection )
    {
        // handle all messages until disconnect.
        try
        {
            //下面是我新加的
            string name = connection.ReadLine();

            lock (lockObj)
            {
                clients.Add(connection);
                clientNames[connection] = name;
            }
            // 以上

            while ( true )
            {
                //下面修改并且新增
                string message = connection.ReadLine();
                string fullMessage = name + ": " + message;

                lock (lockObj)
                {
                    foreach (var client in clients)
                    {
                        client.Send(fullMessage);
                    }
                }
            }   //以上
        }
        catch ( Exception )
        {
            // do anything necessary to handle a disconnected client in here 
            //下面新加的
            lock (lockObj)
            {
                clients.Remove(connection);
                clientNames.Remove(connection);
            }

            connection.Disconnect();
        }
    }
}