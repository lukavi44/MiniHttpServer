using System.Net.Sockets;
using System.Text;

namespace MiniHttpServer.Http
{
    public class HttpServer
    {
        private readonly TcpListener _listener;
        private readonly Router _router;

        public HttpServer(int port, Router router)
        {
            _listener = new TcpListener(System.Net.IPAddress.Any, port);
            _router = router;
        }

        public void Start()
        {
            _listener.Start();

            Console.WriteLine("HTTP Server started on port " + ((System.Net.IPEndPoint)_listener.LocalEndpoint).Port);

            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();

                using NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                string rawRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("----- RAW HTTP REQUEST -----");
                var request = HttpRequestParser.Parse(rawRequest);

                Console.WriteLine("----- PARSED REQUEST -----");
                Console.WriteLine($"Method: {request.Method}");
                Console.WriteLine($"Path: {request.Path}");
                Console.WriteLine($"Version: {request.HttpVersion}");

                foreach (var header in request.Headers)
                {
                    Console.WriteLine($"{header.Key}: {header.Value}");
                }

                HttpResponse response = _router.Handle(request);

                byte[] responseBytes = response.ToBytes();

                stream.Write(responseBytes, 0, responseBytes.Length);

                stream.Write(responseBytes, 0, responseBytes.Length);

                client.Close();
            }
        }
    }
}
