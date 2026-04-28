using MiniHttpServer.Http;

namespace MiniHttpServer;

internal class Program
{
    private static void Main(string[] args)
    {
        var router = new Router();

        router.MapGet("/", request => new HttpResponse
        {
            Body = "Hello from MiniHttpServer!"
        });

        router.MapGet("/users", request => new HttpResponse
        {
            ContentType = "application/json",
            Body = """
                   [
                     { "id": 1, "name": "Luka" },
                     { "id": 2, "name": "Mateja" }
                   ]
                   """
        });

        var server = new HttpServer(5000, router);
        server.Start();
    }
}