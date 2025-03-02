namespace bpqapi.Models;

public readonly record struct MailListEntity
{
    public int Id { get; init; }
    public MonthAndDay Date { get; init; }
    public char Type { get; init; }
    public char State { get; init; }
    public int Length { get; init; }
    public string To { get; init; }
    public string At { get; init; }
    public string From { get; init; }
    public string Subject { get; init; }
}

public readonly record struct MonthAndDay(int Month, int Day)
{
    public static MonthAndDay UtcToday => new(DateTime.UtcNow.Month, DateTime.UtcNow.Day);
}

public record MailEntity
{
    public int Id { get; init; }
    public string? Mid { get; init; }
    public string? Bid { get; init; }
    public MonthAndDay? Date { get; init; }
    public TimeOnly? Time { get; init; }
    public DateTime? DateTime { get; set; }
    public char Type { get; init; }
    public char? State { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
    public required string Subject { get; init; }
    public string? Mbo { get; init; }
    public string? ContentType { get; init; }
    public string? ContentTransferEncoding { get; init; }
    public required string Body { get; init; }
}