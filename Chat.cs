using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SimpleMessengerServer;

public class Chat
{
    private static object _locker = new object();
    public static List<ClientUsername> Clients { get; private set; } = new List<ClientUsername>();

    public void StartServer()
    {
        using var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        var endPoint = IPEndPoint.Parse("127.0.0.1:13350");

        server.Bind(endPoint);
        server.Listen();


        while (true)
        {
            var client = server.Accept();

            var clientUsername = new ClientUsername
            {
                Client = client
            };

            Clients.Add(clientUsername);

            Task.Run(() => ReceiveMessages(clientUsername));
        }
    }

    private void BroadcastMessage(string message, ClientUsername sender)
    {
        foreach (var client in Clients)
        {
            if (client.Client != sender.Client)
            {
                try
                {
                    var sendData = Encoding.UTF8.GetBytes(message);
                    client.Client!.Send(sendData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error broadcasting message to {client.Client!.RemoteEndPoint}. {ex.Message}");
                }
            }
        }
    }

    private void BroadcastMessage(string message)
    {
        foreach (var client in Clients)
        {
            try
            {
                var sendData = Encoding.UTF8.GetBytes(message);
                client.Client!.Send(sendData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting message to {client.Client!.RemoteEndPoint}. {ex.Message}");
            }
        }

    }

    private void BroadcastUsernames()
    {
        var usernameMessage = "2";

        var temp = Clients.Select(x => x.Username);

        List<User> users = new List<User>();

        foreach (var username in temp)
        {
            var user = new User
            {
                UserName = username,
            };
            users.Add(user);
        }

        var content = JsonSerializer.Serialize(users);

        usernameMessage += content;

        BroadcastMessage(usernameMessage);
    }

    private void ReceiveMessages(ClientUsername client)
    {
        byte[] buffer = new byte[2048];

        try
        {
            int byteLength = 0;
            while ((byteLength = client.Client!.Receive(buffer)) > 0)
            {
                var message = Encoding.UTF8.GetString(buffer);

                message = message.Substring(0, byteLength);

                if (message[0] == '2')
                {
                    var resultMessage = message.Substring(1);

                    client.Username = resultMessage;

                    BroadcastUsernames();

                    continue;
                }

                BroadcastMessage(message, client);
            }
        }
        catch
        {
            Clients.Remove(client);
            BroadcastUsernames();
            client.Client!.Close();
        }

    }
}
