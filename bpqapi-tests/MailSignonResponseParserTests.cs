using bpqapi.Parsers;
using FluentAssertions;

namespace bpqapi_tests;

public class MailSignonResponseParserTests
{
    [Fact]
    public void TestPageParsing()
    {
        var html = File.ReadAllText("testdata/mailsignonresponse.html");
        var result = MailSignonResponseParser.Parse(html);
        result.Success.Should().BeTrue();
        result.Input.Should().BeNullOrWhiteSpace();
        result.Value.Should().Be("M0000FB74A0C0");
    }
}