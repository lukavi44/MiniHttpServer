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

        router.MapPost("/users", request =>
        {
            Console.WriteLine("BODY:");
            Console.WriteLine(request.Body);

            return new HttpResponse
            {
                Body = "User received"
            };
        });

        var builder = new MiddlewareBuilder();

        // Logging middleware
        builder.Use(next => request =>
        {
            Console.WriteLine($"[{DateTime.Now}] {request.Method} {request.Path}");

            var response = next(request);

            Console.WriteLine($"Response: {response.StatusCode}");

            return response;
        });

        RequestDelegate pipeline = builder.Build(router.Handle);

        var server = new HttpServer(5000, pipeline);
        server.Start();
    }
}