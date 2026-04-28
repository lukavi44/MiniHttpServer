using System.Text.Json;

namespace MiniHttpServer.Http;

public class HttpRequest
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string HttpVersion { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public Dictionary<string, string> Headers { get; set; } = new();
    public Dictionary<string, string> RouteParameters { get; set; } = new();

    public T? ReadJsonBody<T>()
    {
        if (string.IsNullOrWhiteSpace(Body))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(Body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}