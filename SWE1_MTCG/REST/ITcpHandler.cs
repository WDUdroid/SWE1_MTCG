using System;
using System.IO;

namespace SWE1_MTCG.REST
{
    public interface ITcpHandler : IDisposable
    {
        Stream GetStream();
        void AcceptTcpClient();
        int DataAvailable();
    }
}