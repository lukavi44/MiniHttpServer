using System.Text;

namespace MiniHttpServer.Http;

public class HttpResponse
{
    public int StatusCode { get; set; } = 200;
    public string ReasonPhrase { get; set; } = "OK";
    public string ContentType { get; set; } = "text/plain";
    public string Body { get; set; } = string.Empty;

    public byte[] ToBytes()
    {
        var bodyBytes = Encoding.UTF8.GetBytes(Body);

        var response =
            $"HTTP/1.1 {StatusCode} {ReasonPhrase}\r\n" +
            $"Content-Type: {ContentType}\r\n" +
            $"Content-Length: {bodyBytes.Length}\r\n" +
            "\r\n" +
            Body;

        return Encoding.UTF8.GetBytes(response);
    }
}