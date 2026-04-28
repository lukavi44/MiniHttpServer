namespace MiniHttpServer.Http.Middleware;

public class MiddlewareBuilder
{
    private readonly List<Func<RequestDelegate, RequestDelegate>> _middlewares = new();

    public void Use(Func<RequestDelegate, RequestDelegate> middleware)
    {
        _middlewares.Add(middleware);
    }

    public RequestDelegate Build(RequestDelegate finalHandler)
    {
        RequestDelegate current = finalHandler;

        for (int i = _middlewares.Count - 1; i >= 0; i--)
        {
            current = _middlewares[i](current);
        }

        return current;
    }
}