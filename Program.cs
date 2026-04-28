using MiniHttpServer.Http;

namespace MiniHttpServer;

internal class Program
{
    private static void Main(string[] args)
    {
        HttpServer server = new HttpServer(5000);
        server.Start();
    }
}