using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Http
{
    public class HttpServer
    {
        private readonly TcpListener _listener;

        public HttpServer(int port)
        {
            _listener = new TcpListener(System.Net.IPAddress.Any, port);
        }

        public 
    }
}
