using MiniHttpServer.Http;
using MiniHttpServer.Models;

namespace MiniHttpServer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var router = new Router();

        router.MapGet("/", request => Task.FromResult(new HttpResponse
        {
            Body = "Hello from MiniHttpServer"
        }));

        router.MapGet("/users", request => Task.FromResult(new HttpResponse
        {
            ContentType = "application/json",
            Body = """
                   [
                     { "id": 1, "name": "Luka" },
                     { "id": 2, "name": "Mateja" }
                   ]
                   """
        }));

        //Test conccureny
        router.MapGet("/slow", async request =>
        {
            await Task.Delay(5000);

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
                return Task.FromResult( new HttpResponse
                {
                    StatusCode = 400,
                    ReasonPhrase = "Bad Request",
                    Body = "Name is required"
                });

            }

            return Task.FromResult(new HttpResponse
            {
                ContentType = "application/json",
                Body = $$"""
               {
                 "message": "User created",
                 "name": "{{user.Name}}"
               }
               """
            });
        });

        var builder = new MiddlewareBuilder();

        // Exception handling middleware
        builder.Use(next => async request =>
        {
            try
            {
                return await  next(request);
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
        builder.Use(next => async request =>
        {
            Console.WriteLine($"[{DateTime.Now}] {request.Method} {request.Path}");

            var response = await next(request);

            Console.WriteLine($"Response: {response.StatusCode}");

            return response;
        });

        RequestDelegate pipeline = builder.Build(router.Handle);

        var server = new HttpServer(5000, pipeline);
        await server.StartAsync();
    }
}