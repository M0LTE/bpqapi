namespace bpqapi.Models;

public readonly record struct MailItem
{
    public int Id { get; init; }
    public MonthAndDay Date { get; init; }
    public string State { get; init; }
    public string To { get; init; }
    public string At { get; init; }
    public string From { get; init; }
    public string Subject { get; init; }
    public int Length { get; init; }
}

public readonly record struct MonthAndDay(int Month, int Day);