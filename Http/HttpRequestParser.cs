using System.Text;

namespace MiniHttpServer.Http;

public static class HttpRequestParser
{
    public static HttpRequest Parse(string rawRequest)
    {
        var request = new HttpRequest();

        var lines = rawRequest.Split("\r\n");

        var requestLine = lines[0].Split(' ');

        request.Method = requestLine[0];
        request.Path = requestLine[1];
        request.HttpVersion = requestLine[2];

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
                break;

            var separatorIndex = line.IndexOf(':');

            if (separatorIndex == -1)
                continue;

            var key = line.Substring(0, separatorIndex).Trim();
            var value = line.Substring(separatorIndex + 1).Trim();

            request.Headers[key] = value;
        }

        return request;
    }
}