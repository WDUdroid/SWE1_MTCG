using System;
using System.Collections.Generic;
using System.IO;

namespace SWE1_MTCG.REST
{
    public class WebHandler : IWebHandler
    {
        private readonly ITcpHandler _tcpHandler;

        private IRequestContext _requestContext;

        public Battle BattleCenter = new Battle();

        // used while normal operation
        public WebHandler(ITcpHandler tcpHandler, Battle _battleCenter)
        {
            Console.WriteLine();
            BattleCenter = _battleCenter;
            _tcpHandler = tcpHandler;
            _tcpHandler.AcceptTcpClient();
        }

        // for testing purposes
        public WebHandler(ITcpHandler tcpHandler, IRequestContext requestContext)
        {
            _tcpHandler = tcpHandler;
            _tcpHandler.AcceptTcpClient();

            _requestContext = requestContext;
        }

        // reads message sent by client
        // Streamreader did not work, which made testing a little harder
        // Instead of getting .DataAvailable() from the StreamReader-Object,
        // I had to check, if the TcpClient has available data.
        public string GetHttpContent()
        {
            //NetworkStream stream = _tcpHandler.GetStream();
            //StreamReader reader = new StreamReader(stream);
            var stream = _tcpHandler.GetStream();
            var receivedData = "";

            while (_tcpHandler.DataAvailable() != 0)
            { 
                Byte[] bytes = new Byte[4096];
                int i = stream.Read(bytes, 0, bytes.Length);
                receivedData += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            }

            return receivedData;
        }

        public void WorkHttpRequest(string content, List<String> messageData)
        {
            _requestContext = new RequestContext(content, messageData, BattleCenter);
            _requestContext.RequestCoordinator();
        }

        public void SendHttpContent()
        {
            var response = "HTTP/1.1" + " " + _requestContext.StatusCode + "\r\n"
                       + "Server: " + "MTCG-Server" + "\r\n";

            if (_requestContext.Payload != "")
            {
                response += _requestContext.ContentType + "\r\n";

               
                var mlength = _requestContext.Payload.Length;
                response += "Content-Length: " + mlength + "\r\n\r\n" + _requestContext.Payload;
                
            }
            using StreamWriter writer = new StreamWriter(_tcpHandler.GetStream()) { AutoFlush = true };
            writer.WriteLine(response);
            Console.WriteLine(response);
        }
    }
}
