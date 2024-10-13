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
                var text = a.NextSibling.InnerText.Trim();

                // 06-Oct BN 26386 NTS     ARRL    CX2SA   October 2024 NTS Letter

                var id = a.InnerText;
                var day = text[..2];
                var month = text[3..6];
                var state = text[7..9].Trim();
                var len = text[10..15];
                var to = text[16..24];
                var at = text[24..32];
                var from = text[32..40];
                var subject = text[40..];

                var mailItem = new MailListEntity
                {
                    Id = int.Parse(id),
                    Date = new MonthAndDay(months[month], int.Parse(day)),
                    Type = state[0],
                    State = state[1],
                    Length = int.Parse(len),
                    To = to.Trim(),
                    At = at.Trim(),
                    From = from.Trim(),
                    Subject = subject.Trim()
                };

                mailItems.Add(mailItem);
            }

            return ParseResult<(string token, MailListEntity[] mail)>.CreateSuccess((token, mailItems.ToArray()));
        }
        catch (Exception ex)
        {
            return ParseResult<(string token, MailListEntity[] mail)>.CreateFailure(ex, html);
        }
    }

    internal static ParseResult<bool> ParseMaybeLoginPage(string html)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText;

            if (title == "BPQ32 Mail Server Access" || title == "WebMail")
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