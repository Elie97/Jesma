using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace SimulationVéhicule
{
    public delegate void ConnectionEvent(object sender, TcpClient user);
    public delegate void DataReceivedEvent(TcpClient sender, byte[] data);
}
