using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SimpleMessengerServer;

internal class Program
{

    static void Main(string[] args)
    {
        var chat = new Chat();

        chat.StartServer();
    }

    
}
