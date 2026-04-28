namespace MiniHttpServer.Http.Core;

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

        int emptyLineIndex = Array.IndexOf(lines, "");

        if (emptyLineIndex != -1 && emptyLineIndex < lines.Length - 1)
        {
            var bodyLines = lines.Skip(emptyLineIndex + 1);
            request.Body = string.Join("\r\n", bodyLines);
        }

        return request;
    }
}