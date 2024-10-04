using bpqapi.Models;
using HtmlAgilityPack;

namespace bpqapi.Parsers;

public static class MailForwardingPageParser
{
    public static ForwardingOptions ParseOptions(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new ForwardingOptions
        {
            Aliases = doc.DocumentNode.SelectNodes("//textarea").WithName("Aliases").InnerText.LinesToArray()
        };

        return result;
    }

    public static string[] ParsePartnerList(string postRequestToFwdList)
    {
        throw new NotImplementedException();
    }

    public static ForwardingStation ParseStationResponse(string postRequestToFwdDetails)
    {
        throw new NotImplementedException();
    }
}

internal static class Extensions
{
    public static string[] LinesToArray(this string str) 
        => str.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

    public static HtmlNode WithName(this HtmlNodeCollection nodes, string attributeValue) 
        => nodes.WithAttribute("name", attributeValue);

    public static HtmlNode WithAttribute(this HtmlNodeCollection nodes, string attributeName, string attributeValue) 
        => nodes.Single(n => n.Attributes.Any(att => string.Equals(att.Name, attributeName, StringComparison.OrdinalIgnoreCase) && att.Value == attributeValue));
}