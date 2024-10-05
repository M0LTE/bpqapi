namespace bpqapi.Parsers;

public record struct ParseResult<T> where T : notnull
{
    public static ParseResult<T> CreateSuccess(T value) => new() { Success = true, Value = value };

    public static ParseResult<T> CreateFailure(Exception ex, string html) => new() { Success = false, Exception = ex, Input = html };

    public readonly T EnsureSuccess()
    {
        if (!Success)
        {
            var temp = Path.GetTempFileName();
            File.WriteAllText(temp, Input);
            throw new Exception($"{typeof(T).Name} parsing failed. Input saved: {temp}", Exception);
        }

        return Value;
    }

    public bool Success { get; set; }
    public T Value { get; set; }
    public Exception? Exception { get; set; }
    public string Input { get; set; }
}
