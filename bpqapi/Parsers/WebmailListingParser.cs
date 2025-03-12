using bpqapi.Models;
using bpqapi.Services;
using HtmlAgilityPack;
using System.Text;

namespace bpqapi.Parsers;

public class WebmailListingParser
{
    public static ParseResult<(string token, MailListEntity[] mail)> Parse(string html)
    {
        if (html.Contains("Please enter Callsign and Password to access WebMail"))
        {
            return ParseResult<(string token, MailListEntity[] mail)>.CreateFailure(new LoginFailedException(), html);
        }

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

    private enum ParseState
    {
        Day,
        Month,
        State,
        LookingForLength,
        Length,
        To,
        At,
        From,
        Subject
    }

    public static MailListEntity ParseMailItem(string text, int id)
    {
        // 06-Oct BN 26386 NTS     ARRL    CX2SA   October 2024 NTS Letter
        // 03-Jun BF  3731 KEP     WW      NC8Q    
        // 03-Jun BF  3731 KEP     WW      NC8Q

        string month = "", day = "", state = "", len = "", to = "", at = "", from = "";

        var parseState = ParseState.Day;
        var sb = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            var c = text[i];

            if (parseState == ParseState.Day)
            {
                if (char.IsDigit(c))
                {
                    parseState = ParseState.Month;
                    day = new string([c, text[i + 1]]);
                    i++;
                }
            }
            else if (parseState == ParseState.Month)
            {
                if (c == '-')
                {
                    continue;
                }
                else if (char.IsLetter(c))
                {
                    month = new string([c, text[i + 1], text[i + 2]]);
                    i += 3;
                    parseState = ParseState.State;
                }
            }
            else if (parseState == ParseState.State)
            {
                if (char.IsLetter(c))
                {
                    state = new string([c, text[i + 1]]);
                    i += 2;
                    parseState = ParseState.LookingForLength;
                }
            }
            else if (parseState == ParseState.LookingForLength)
            {
                if (char.IsDigit(c))
                {
                    parseState = ParseState.Length;
                    sb.Clear();
                    sb.Append(c);
                }
            }
            else if (parseState == ParseState.Length)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
                else if (c == ' ')
                {
                    len = sb.ToString();
                    sb.Clear();
                    parseState = ParseState.To;
                }
            }
            else if (parseState == ParseState.To)
            {
                to = new string([text[i], text[i + 1], text[i + 2], text[i + 3], text[i + 4], text[i + 5]]);
                parseState = ParseState.At;
                i += 7;
            }
            else if (parseState == ParseState.At)
            {
                at = new string([text[i], text[i + 1], text[i + 2], text[i + 3], text[i + 4], text[i + 5]]);
                parseState = ParseState.From;
                i += 7;
            }
            else if (parseState == ParseState.From)
            {
                from = new string([text[i], text[i + 1], text[i + 2], text[i + 3], text[i + 4], text[i + 5]]);
                parseState = ParseState.Subject;
                i += 7;
                sb.Clear();
            }
            else if (parseState == ParseState.Subject)
            {
                sb.Append(c);
            }
        }

        var result = new MailListEntity
        {
            Id = id,
            Date = new MonthAndDay(months[month], int.Parse(day)),
            Type = state[0],
            State = state[1],
            Length = int.Parse(len),
            To = to.Trim(),
            At = at.Trim(),
            From = from.Trim(),
            Subject = sb.ToString().Trim()
        };

        return result;
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