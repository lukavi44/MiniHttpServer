using MiniHttpServer.Http;
using MiniHttpServer.Http.Core;
using MiniHttpServer.Http.Middleware;
using MiniHttpServer.Models;

namespace MiniHttpServer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var router = new Router();

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

        router.MapGet("/users/{id}", request =>
        {
            var id = request.RouteParameters["id"];

            return Task.FromResult(new HttpResponse
            {
                ContentType = "application/json",
                Body = $$"""
               {
                 "id": "{{id}}",
                 "name": "User {{id}}"
               }
               """
            });
        });

        static string GetContentType(string path)
        {
            return Path.GetExtension(path) switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                _ => "text/plain"
            };
        }

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

        builder.Use(next => async request =>
        {
            var path = request.Path == "/"
                ? "/index.html"
                : request.Path;

            var filePath = "wwwroot" + path;

            if (File.Exists(filePath))
            {
                var content = await File.ReadAllTextAsync(filePath);

                return new HttpResponse
                {
                    ContentType = GetContentType(filePath),
                    Body = content
                };
            }

            return await next(request);
        });

        RequestDelegate pipeline = builder.Build(router.Handle);

        var server = new HttpServer(5000, pipeline);
        await server.StartAsync();
    }
}