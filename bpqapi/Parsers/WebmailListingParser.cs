using bpqapi.Models;
using HtmlAgilityPack;

namespace bpqapi.Parsers;

public class WebmailListingParser
{
    public static ParseResult<(string token, MailListEntity[] mail)> Parse(string html)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var token = doc.DocumentNode.SelectNodes("//a").First(n => n.GetAttributeValue("href", "").StartsWith("/WebMail/")).GetAttributeValue("href", "").Split('?').Last();

            var mailItems = new List<MailListEntity>();

            var anchors = doc.DocumentNode.SelectNodes("//div[@id='main']/pre/a");

            var year = DateTime.UtcNow.Year;
            foreach (var a in anchors)
            {
                var mailItem = ParseMailItem(a.NextSibling.InnerText, int.Parse(a.InnerText));
                mailItems.Add(mailItem);
            }

            return ParseResult<(string token, MailListEntity[] mail)>.CreateSuccess((token, mailItems.ToArray()));
        }
        catch (Exception ex)
        {
            return ParseResult<(string token, MailListEntity[] mail)>.CreateFailure(ex, html);
        }
    }

    public static MailListEntity ParseMailItem(string text, int id)
    {
        // 06-Oct BN 26386 NTS     ARRL    CX2SA   October 2024 NTS Letter
        // 03-Jun BF  3731 KEP     WW      NC8Q    

        var day = text[1..3];
        var month = text[4..7];
        var state = text[8..10].Trim();
        var len = text[11..16];
        var to = text[17..25];
        var at = text[25..33];
        var from = text[33..41];
        var subject = text[41..];

        return new MailListEntity
        {
            Id = id,
            Date = new MonthAndDay(months[month], int.Parse(day)),
            Type = state[0],
            State = state[1],
            Length = int.Parse(len),
            To = to.Trim(),
            At = at.Trim(),
            From = from.Trim(),
            Subject = subject.Trim()
        };
    }

    internal static ParseResult<bool> ParseMaybeLoginPage(string html)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText;

            if (title == "BPQ32 Mail Server Access")
            {
                return ParseResult<bool>.CreateSuccess(true); // is login page
            }
            else
            {
                return ParseResult<bool>.CreateSuccess(false); // is not login page
            }
        }
        catch (Exception ex)
        {
            return ParseResult<bool>.CreateFailure(ex, html);
        }
    }

    private static readonly Dictionary<string, int> months = new()
    {
        ["Jan"] = 1,
        ["Feb"] = 2,
        ["Mar"] = 3,
        ["Apr"] = 4,
        ["May"] = 5,
        ["Jun"] = 6,
        ["Jul"] = 7,
        ["Aug"] = 8,
        ["Sep"] = 9,
        ["Oct"] = 10,
        ["Nov"] = 11,
        ["Dec"] = 12
    };
}