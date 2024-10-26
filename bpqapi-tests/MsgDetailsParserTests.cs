using bpqapi.Parsers;
using FluentAssertions;

namespace bpqapi_tests;

public class MsgDetailsParserTests
{
    [Fact]
    public void TestPageParsing()
    {
        var html = File.ReadAllText("testdata/msgdetails-response.html");
        /*var result = MsgDetailsResponseParser.Parse(html);
        result.Success.Should().BeTrue();
        result.Input.Should().BeNullOrWhiteSpace();
        var options = result.Value;*/
        
    }
}