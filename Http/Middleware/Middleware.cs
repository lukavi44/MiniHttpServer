using MiniHttpServer.Http.Core;

namespace MiniHttpServer.Http.Middleware;

public delegate Task<HttpResponse> RequestDelegate(HttpRequest request);