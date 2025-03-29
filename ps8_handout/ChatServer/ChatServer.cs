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
    /// <summary>
    /// Stores all currently connected clients.
    /// </summary>
    private static readonly List<NetworkConnection> clients = new();

    /// <summary>
    /// Maps each client connection to its corresponding username.
    /// </summary>
    private static readonly Dictionary<NetworkConnection, string> clientNames = new();

    /// <summary>
    /// An object used to lock shared resources for thread safety.
    /// </summary>
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
    /// <param name="connection">The client's network connection.</param>
    private static void HandleConnect( NetworkConnection connection )
    {
        // handle all messages until disconnect.
        try
        {
            // Receive the first message, treat it as the username
            string name = connection.ReadLine();

            lock (lockObj)
            {
                // Add client to the connection list and name map
                clients.Add(connection);
                clientNames[connection] = name;
            }

            while ( true )
            {
                string message = connection.ReadLine();
                string fullMessage = name + ": " + message;

                lock (lockObj)
                {
                    // Broadcast the message to all clients
                    foreach (var client in clients)
                    {
                        client.Send(fullMessage);
                    }
                }
            }
        }
        catch ( Exception )
        {
            // do anything necessary to handle a disconnected client in here 
            //Handle disconnection: remove the client from server state
            lock (lockObj)
            {
                clients.Remove(connection);
                clientNames.Remove(connection);
            }

            connection.Disconnect();
        }
    }
}