namespace MiniHttpServer.Http;

public class HttpRequest
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string HttpVersion { get; set; } = string.Empty;

    public Dictionary<string, string> Headers { get; set; } = new();
}