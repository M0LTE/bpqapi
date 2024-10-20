using HtmlAgilityPack;

namespace bpqapi.Parsers;

public static class MsgDetailsResponseParser
{
    public static ParseResult<MessageDetails> Parse(int messageId, string html)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tds = doc.DocumentNode.SelectNodes("//td").WithAttribute("onclick", a => a.StartsWith("ck(\""));

            var dict = new Dictionary<string, MessageDetails.QueueStatus>();

            foreach (var item in tds)
            {
                var colour = item.GetAttributeValue("style", "").Replace("background-color: ", "").Replace(";", "");
                dict.Add(item.InnerText, Map[colour]);
            }

            var result = new MessageDetails
            {
                ForwardingStatus = dict,
                Id = messageId
            };

            return ParseResult<MessageDetails>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ParseResult<MessageDetails>.CreateFailure(ex, html);
        }
    }

    private static readonly Dictionary<string, MessageDetails.QueueStatus> Map = new()
    {
        { "#FFFFFF", MessageDetails.QueueStatus.Unqueued },
        { "#FFFF00", MessageDetails.QueueStatus.Pending },
        { "#98FFA0", MessageDetails.QueueStatus.Sent }
    };
}

public readonly record struct MessageDetails
{
    public int Id { get; init; }
    public Dictionary<string, QueueStatus> ForwardingStatus { get; init; }

    public enum QueueStatus
    {
        Unqueued, Pending, Sent
    }
}