using HtmlAgilityPack;

namespace bpqapi.Parsers;

public class MailManagementSignonResponseParser
{
    public static ParseResult<string> Parse(string html)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = doc.DocumentNode.SelectNodes("//a").First().GetAttributeValue("href", "").Split('?').Last();

            return ParseResult<string>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ParseResult<string>.CreateFailure(ex, html);
        }
    }
}
