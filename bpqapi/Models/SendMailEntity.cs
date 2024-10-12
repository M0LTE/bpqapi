namespace bpqapi.Models;

public readonly record struct SendMailEntity
{
    public string To { get; init; }
    public string Subject { get; init; }
    public char Type { get; init; }
    public string Bid { get; init; }
    public string Body { get; init; }
}
