using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MiniHttpServer.Http
{
    public class HttpServer
    {
        private readonly TcpListener _listener;
        private readonly RequestDelegate _middlewarePipeline;

        public HttpServer(int port, RequestDelegate pipeline)
        {
            _listener = new TcpListener(System.Net.IPAddress.Any, port);
            _middlewarePipeline = pipeline;
        }

        public async Task StartAsync()
        {
            _listener.Start();

            Console.WriteLine("HTTP Server started on port " + ((IPEndPoint)_listener.LocalEndpoint).Port);

            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();

                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                string rawRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("----- RAW HTTP REQUEST -----");
                Console.WriteLine(rawRequest);

                var request = HttpRequestParser.Parse(rawRequest);

                Console.WriteLine("----- PARSED REQUEST -----");
                Console.WriteLine($"Method: {request.Method}");
                Console.WriteLine($"Path: {request.Path}");
                Console.WriteLine($"Version: {request.HttpVersion}");

                foreach (var header in request.Headers)
                {
                    Console.WriteLine($"{header.Key}: {header.Value}");
                }

                HttpResponse response = await _middlewarePipeline(request);
                byte[] responseBytes = response.ToBytes();

                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}
