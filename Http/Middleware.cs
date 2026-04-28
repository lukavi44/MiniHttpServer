namespace MiniHttpServer.Http;

public delegate Task<HttpResponse> RequestDelegate(HttpRequest request);