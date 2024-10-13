namespace bpqapi.Parsers;

public static class DebugWriter
{
    public static void LogAndThrow<T>(string html, Exception? ex)
    {
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, html);
        throw new Exception($"{typeof(T).Name} parsing failed. Input saved: {temp}", ex);
    }
}