using MiniHttpServer.Http;

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

    public async Task<HttpResponse> Handle(HttpRequest request)
    {
        foreach (var route in _routes)
        {
            var routeParts = route.Key.Split(' ', 2);

            var method = routeParts[0];
            var routePath = routeParts[1];

            if (!string.Equals(method, request.Method, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (TryMatchRoute(routePath, request.Path, out var parameters))
            {
                request.RouteParameters = parameters;
                return await route.Value(request);
            }
        }

        return new HttpResponse
        {
            StatusCode = 404,
            ReasonPhrase = "Not Found",
            Body = "404 Not Found"
        };
    }

    private static bool TryMatchRoute(
        string routePath,
        string requestPath,
        out Dictionary<string, string> parameters)
    {
        parameters = new Dictionary<string, string>();

        var routeParts = routePath.Trim('/').Split('/');
        var requestParts = requestPath.Trim('/').Split('/');

        if (routeParts.Length != requestParts.Length)
        {
            return false;
        }

        for (int i = 0; i < routeParts.Length; i++)
        {
            var routePart = routeParts[i];
            var requestPart = requestParts[i];

            if (routePart.StartsWith("{") && routePart.EndsWith("}"))
            {
                var parameterName = routePart.Trim('{', '}');
                parameters[parameterName] = requestPart;
                continue;
            }

            if (!string.Equals(routePart, requestPart, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}