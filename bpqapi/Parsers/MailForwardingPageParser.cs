using bpqapi.Models;
using HtmlAgilityPack;

namespace bpqapi.Parsers;

public static class MailForwardingPageParser
{
    public static ParseResult<ForwardingOptions> ParseOptions(string html)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = new ForwardingOptions
            {
                MaxSizeToSend = doc.DocumentNode.SelectNodes("//input").WithName("MaxTX").Value().AsInt(),
                MaxSizeToReceive = doc.DocumentNode.SelectNodes("//input").WithName("MaxRX").Value().AsInt(),
                MaxAgeForBulls = TimeSpan.FromDays(doc.DocumentNode.SelectNodes("//input").WithName("MaxAge").Value().AsInt()),
                WarnIfNoRouteForPOrT = doc.DocumentNode.SelectNodes("//input").WithName("WarnNoRoute").GetCheckboxChecked(),
                UseLocalTime = doc.DocumentNode.SelectNodes("//input").WithName("LocalTime").GetCheckboxChecked(),
                SendPMessagesToMoreThanOneBbs = doc.DocumentNode.SelectNodes("//input").WithName("SendPtoMultiple").GetCheckboxChecked(),
                Use4CharContinentCodes = doc.DocumentNode.SelectNodes("//input").WithName("FourCharCont").GetCheckboxChecked(),
                Aliases = doc.DocumentNode.SelectNodes("//textarea").WithName("Aliases").InnerText.LinesToArray()
            };

            return ParseResult<ForwardingOptions>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ParseResult<ForwardingOptions>.CreateFailure(ex, html);
        }
    }

    public static ParseResult<string[]> ParsePartnerList(string apiResponse)
    {
        try
        {
            var parsed = apiResponse.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).SkipLast(1).ToArray();
            return ParseResult<string[]>.CreateSuccess(parsed);
        }
        catch (Exception ex)
        {
            return ParseResult<string[]>.CreateFailure(ex, apiResponse);
        }
    }

    public static ParseResult<ForwardingStation> ParseStationResponse(string html)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = new ForwardingStation
            {
                Callsign = doc.DocumentNode.SelectNodes("//h3").First().InnerText.Split('-').First().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Last(),
                QueueLength = doc.DocumentNode.SelectNodes("//h3").First().InnerText.Split('-').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First().Trim().AsInt(),
                ForwardingConfig = new ForwardingConfig
                {
                    To = doc.DocumentNode.SelectNodes("//textarea").WithName("TO").InnerText.LinesToArray(),
                    At = doc.DocumentNode.SelectNodes("//textarea").WithName("AT").InnerText.LinesToArray(),
                    Times = doc.DocumentNode.SelectNodes("//textarea").WithName("Times").InnerText.LinesToArray(),
                    ConnectScript = doc.DocumentNode.SelectNodes("//textarea").WithName("FWD").InnerText.LinesToArray(),
                    HierarchicalRoutes = doc.DocumentNode.SelectNodes("//textarea").WithName("HRB").InnerText.LinesToArray(),
                    HR = doc.DocumentNode.SelectNodes("//textarea").WithName("HRP").InnerText.LinesToArray(),
                    BbsHa = doc.DocumentNode.SelectNodes("//input").WithName("BBSHA").Value(),
                    EnableForwarding = doc.DocumentNode.SelectNodes("//input").WithName("EnF").GetCheckboxChecked(),
                    ForwardingInterval = TimeSpan.FromSeconds(doc.DocumentNode.SelectNodes("//input").WithName("Interval").Value().AsInt()),
                    RequestReverse = doc.DocumentNode.SelectNodes("//input").WithName("EnR").GetCheckboxChecked(),
                    ReverseInterval = TimeSpan.FromSeconds(doc.DocumentNode.SelectNodes("//input").WithName("RInterval").Value().AsInt()),
                    SendNewMessagesWithoutWaiting = doc.DocumentNode.SelectNodes("//input").WithName("NoWait").GetCheckboxChecked(),
                    FbbBlocked = doc.DocumentNode.SelectNodes("//input").WithName("Blocked").GetCheckboxChecked(),
                    MaxBlock = doc.DocumentNode.SelectNodes("//input").WithName("FBBBlock").Value().AsInt(),
                    SendPersonalMailOnly = doc.DocumentNode.SelectNodes("//input").WithName("Personal").GetCheckboxChecked(),
                    AllowBinary = doc.DocumentNode.SelectNodes("//input").WithName("Bin").GetCheckboxChecked(),
                    UseB1Protocol = doc.DocumentNode.SelectNodes("//input").WithName("B1").GetCheckboxChecked(),
                    UseB2Protocol = doc.DocumentNode.SelectNodes("//input").WithName("B2").GetCheckboxChecked(),
                    SendCtrlZInsteadOfEx = doc.DocumentNode.SelectNodes("//input").WithName("CTRLZ").GetCheckboxChecked(),
                    IncomingConnectTimeout = TimeSpan.FromSeconds(doc.DocumentNode.SelectNodes("//input").WithName("ConTimeOut").Value().AsInt())
                }
            };

            return ParseResult<ForwardingStation>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ParseResult<ForwardingStation>.CreateFailure(ex, html);
        }
    }
}

internal static class Extensions
{
    public static bool GetCheckboxChecked(this HtmlNode node) => node.GetAttributeValue("checked", "") == "checked";
    public static string Value(this HtmlNode node) => node.Attributes["value"].Value;

    public static int AsInt(this string str) => int.Parse(str);
    public static string[] LinesToArray(this string str)
        => str.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

    public static HtmlNode WithName(this HtmlNodeCollection nodes, string attributeValue)
        => nodes.WithAttribute("name", attributeValue);

    public static HtmlNode WithAttribute(this HtmlNodeCollection nodes, string attributeName, string attributeValue)
        => nodes.Single(n => n.Attributes.Any(att => string.Equals(att.Name, attributeName, StringComparison.OrdinalIgnoreCase) && att.Value == attributeValue));

    public static IEnumerable<HtmlNode> WithAttribute(this HtmlNodeCollection nodes, string attributeName, Func<string, bool> predicate)
        => nodes.Where(n => n.Attributes.Any(att => string.Equals(att.Name, attributeName, StringComparison.OrdinalIgnoreCase) && predicate(att.Value)));
}