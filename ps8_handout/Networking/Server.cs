// <copyright file="Server.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace CS3500.Networking;

/// <summary>
///   Represents a server task that waits for connections on a given
///   port and calls the provided delegate when a connection is made.
/// </summary>
public static class Server
{
    /// <summary>
    ///   Wait on a TcpListener for new connections. Alert the main program
    ///   via a callback (delegate) mechanism.
    /// </summary>
    /// <param name="handleConnect">
    ///   Handler for what the user wants to do when a connection is made.
    ///   This should be run asynchronously via a new thread.
    /// </param>
    /// <param name="port"> The port (e.g., 11000) to listen on. </param>
    public static void StartServer( Action<NetworkConnection> handleConnect, int port )
    {
        // Create a listener that accepts connections on any IP address at the given port
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        // Start listening for incoming connection requests
        listener.Start();
        new Thread(() =>
        {
            while (true)
            {
                try
                {
                    // Block until a client connects, then accept the connection
                    TcpClient client = listener.AcceptTcpClient();
                    // Wrap the raw TcpClient with our NetworkConnection abstraction
                    NetworkConnection connection = new NetworkConnection(client);
                    // Start a new thread to handle communication with this client
                    new Thread(() => handleConnect(connection)).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error accepting client: " + e.Message);
                }
            }
        }).Start();
    }
}
