using SQLite;

namespace bpqapi.Models;

[Table("mail")]
internal sealed class DbMail
{
    [PrimaryKey, NotNull]
    public int Id { get; init; }
    public string State { get; init; } = "";
    public string? Mid { get; init; }
    public string? Bid { get; init; }
    public DateTime DateTime { get; set; }
    public string Type { get; init; } = "";
    public string From { get; init; } = "";
    public string To { get; init; } = "";
    public string Subject { get; init; } = "";
    public string? Mbo { get; init; }
    public string? ContentType { get; init; }
    public string? ContentTransferEncoding { get; init; }
    public string Body { get; init; } = "";

    /// <summary>
    /// This is to store the mail client's read status.
    /// </summary>
    public bool Read { get; init; }
}