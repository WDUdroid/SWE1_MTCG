using System;
using System.IO;
using System.Net.Sockets;

namespace SWE1_MTCG.REST
{
    public interface ITcpHandler
    {
        Stream GetStream(TcpClient client);
        TcpClient AcceptTcpClient();
        int DataAvailable(TcpClient client);
    }
}