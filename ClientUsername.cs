using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace SimpleMessengerServer;

public class ClientUsername
{
    public string? Username { get; set; }
    public Socket? Client { get; set; }
}
