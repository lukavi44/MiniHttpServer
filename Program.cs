using MiniHttpServer.Http;
using MiniHttpServer.Models;

namespace MiniHttpServer;

internal class Program
{
    private static async Task Main(string[] args)
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

        //Test conccureny
        router.MapGet("/slow", request =>
        {
            Thread.Sleep(10000);

            return new HttpResponse
            {
                Body = "Slow response finished"
            };
        });

        router.MapPost("/users", request =>
        {
            User? user = request.ReadJsonBody<User>();

            if (user is null || string.IsNullOrWhiteSpace(user.Name))
            {
                return new HttpResponse
                {
                    StatusCode = 400,
                    ReasonPhrase = "Bad Request",
                    Body = "Name is required"
                };
            }

            return new HttpResponse
            {
                ContentType = "application/json",
                Body = $$"""
               {
                 "message": "User created",
                 "name": "{{user.Name}}"
               }
               """
            };
        });

        var builder = new MiddlewareBuilder();

        // Exception handling middleware
        builder.Use(next => request =>
        {
            try
            {
                return next(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex);

                return new HttpResponse
                {
                    StatusCode = 500,
                    ReasonPhrase = "Internal Server Error",
                    Body = "Internal Server Error"
                };
            }
        });

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
        await server.StartAsync();
    }
}