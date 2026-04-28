namespace MiniHttpServer.Http;

public class Router
{
    private readonly Dictionary<string, Func<HttpRequest, Task<HttpResponse>>> _routes = new();
    public void MapGet(string path, Func<HttpRequest, Task<HttpResponse>> handler)
    {
        _routes[$"GET {path}"] = handler;
    }

    public void MapPost(string path, Func<HttpRequest, Task<HttpResponse>> handler)
    {
        _routes[$"POST {path}"] = handler;
    }

    public Task<HttpResponse> Handle(HttpRequest request)
    {
        var key = $"{request.Method} {request.Path}";

        if (_routes.TryGetValue(key, out var handler))
        {
            return handler(request);
        }

        return Task.FromResult(new HttpResponse
        {
            StatusCode = 404,
            ReasonPhrase = "Not Found",
            Body = "404 Not Found"
        });
    }
}