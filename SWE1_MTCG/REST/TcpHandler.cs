using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SWE1_MTCG.REST
{
    class TcpHandler : ITcpHandler
    {
        private TcpListener _server;
        public TcpClient _client;

        public TcpHandler()
        {
            // Connect server
            _server = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 10001);
            _server.Start(5);
        }

        public void AcceptTcpClient()
        {
            Console.WriteLine(">>>>>>>>>>Waiting for a client<<<<<<<<<<");
            Console.WriteLine("...");

            _client = _server.AcceptTcpClient();

            Console.WriteLine(">>Servicing client");
            Console.WriteLine("...");
        }

        public Stream GetStream() => _client.GetStream();
        public void CloseClient() => _client.Close();

        public void Stop() => _server.Stop();

        public int DataAvailable() => _client.Available;

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
