namespace MiniHttpServer.Http;

public class Router
{
    private readonly Dictionary<string, Func<HttpRequest, HttpResponse>> _routes = new();

    public void MapGet(string path, Func<HttpRequest, HttpResponse> handler)
    {
        _routes[$"GET {path}"] = handler;
    }

    public HttpResponse Handle(HttpRequest request)
    {
        var key = $"{request.Method} {request.Path}";

        if (_routes.TryGetValue(key, out var handler))
        {
            return handler(request);
        }

        return new HttpResponse
        {
            StatusCode = 404,
            ReasonPhrase = "Not Found",
            Body = "404 Not Found"
        };
    }
}