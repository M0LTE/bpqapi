using bpqapi.Models;
using HtmlAgilityPack;
using System.Globalization;
using System.Text;

namespace bpqapi.Parsers;

public class WebmailParser
{
    public static ParseResult<MailEntity> Parse(string html, ILogger logger)
    {
        static string? GetField(string[] lines, string fieldName)
        {
            var line = lines.FirstOrDefault(line => line.StartsWith(fieldName));
            if (line == null)
            {
                return null;
            }

            return line[fieldName.Length..];
        }

        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var text = doc.DocumentNode.SelectSingleNode("//*[@id=\"txt\"]").InnerText;
            var lines = text.Split('\r', '\n');
            var from = GetField(lines, "From: ");
            var to = GetField(lines, "To: ");
            var typeStatus = GetField(lines, "Type/Status: ");
            var type = GetField(lines, "Type: ");
            var dateTime = GetField(lines, "Date/Time: ");
            var date = GetField(lines, "Date: ");
            var mid = GetField(lines, "MID: ");

            DateTime dt;
            if (dateTime != null)
            {
                var parsed = DateTime.ParseExact(dateTime, "dd-MMM HH:mmZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                var year = parsed.Month <= DateTime.UtcNow.Month ? 2025 : 2024; // hack for now.
                dt = new DateTime(year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, 0, DateTimeKind.Utc);
                logger.LogInformation("Assuming dateTime {input} means {output:yyyy-MM-dd HH:mm}", dateTime, dt);
            }
            else
            {
                dt = DateTime.ParseExact(date!, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                logger.LogInformation("Assuming dt {input} means {output:yyyy-MM-dd HH:mm}", dateTime, dt);
            }

            var bid = GetField(lines, "Bid: ");
            var title = GetField(lines, "Title: ");
            var subject = GetField(lines, "Subject: ");

            var found = false;
            var sb = new StringBuilder();
            using var stringWriter = new StringWriter(sb) { NewLine = "\n" };

            var footerLine = lines.Where(line => !string.IsNullOrWhiteSpace(line)).Last();

            foreach (var line in lines)
            {
                if (!found && string.IsNullOrWhiteSpace(line))
                {
                    found = true;
                    continue;
                }

                if (found)
                {
                    if (line != footerLine)
                    {
                        stringWriter.WriteLine(line);
                    }
                }
            }
            var body = sb.ToString();

            var mailItem = new MailEntity
            {
                Id = int.Parse(doc.DocumentNode.SelectSingleNode("//body/h3").InnerText.Split(" ").Last()),
                From = from!,
                To = to!,
                State = typeStatus == null ? null : typeStatus.Length == 2 ? typeStatus[1] : null,
                Type = type != null ? type[0] : typeStatus![0],
                Date = new MonthAndDay(dt.Month, dt.Day),
                Time = new TimeOnly(dt.Hour, dt.Minute),
                DateTime = dt,
                Bid = bid,
                Mid = mid,
                Subject = title ?? subject!,
                Body = body.TrimEnd(),
                Mbo = GetField(lines, "Mbo: "),
                ContentType = GetField(lines, "Content-Type: "),
                ContentTransferEncoding = GetField(lines, "Content-Transfer-Encoding: ")
            };

            return ParseResult<MailEntity>.CreateSuccess(mailItem);
        }
        catch (Exception ex)
        {
            return ParseResult<MailEntity>.CreateFailure(ex, html);
        }
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