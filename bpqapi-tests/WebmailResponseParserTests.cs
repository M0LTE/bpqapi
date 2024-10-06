using bpqapi.Parsers;
using FluentAssertions;

namespace bpqapi_tests;

public class WebmailResponseParserTests
{
    [Fact]
    public void TestPageParsing()
    {
        var html = File.ReadAllText("testdata/webmaillistingpage.html");
        var result = WebmailListingParser.Parse(html);
        result.Success.Should().BeTrue();
        result.Input.Should().BeNullOrWhiteSpace();

        var (token, mail) = result.Value;

        mail[0].Id.Should().Be(6876);
        mail[0].From.Should().Be("CX2SA");
        mail[0].To.Should().Be("NTS");
        mail[0].At.Should().Be("ARRL");
        mail[0].Date.Month.Should().Be(10);
        mail[0].Date.Day.Should().Be(16);
        mail[0].State.Should().Be("BN");
        mail[0].Length.Should().Be(26386);
        mail[0].Subject.Should().Be("October 2024 NTS Letter");

        mail.Should().HaveCount(221);

        mail.Last().Id.Should().Be(1111);
        mail.Last().To.Should().Be("NTSxxxx");
        mail.Last().At.Should().Be("ARRLxxx");
        mail.Last().From.Should().Be("CX2SAxx");
    }
}
