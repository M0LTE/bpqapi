using bpqapi.Models;
using bpqapi.Parsers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace bpqapi_tests;

public class WebmailParserTests
{
    [Fact]
    public void ParseMailPage01()
    {
        var html = File.ReadAllText("TestData/mailsample-01.html");
        var result = WebmailParser.Parse(html, Mock.Of<ILogger>());

        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Input.Should().BeNull();

        var mail = result.Value;
        
        mail.Id.Should().Be(6925);
        mail.From.Should().Be("M0LTE@GB7RDG.#42.GBR.EURO");
        mail.To.Should().Be("2E0JJI");
        mail.State.Should().Be('F');
        mail.Type.Should().Be('P');
        mail.Date.Should().Be(new MonthAndDay(10, 12));
        mail.Time.Should().Be(new TimeOnly(13, 20));
        mail.Bid.Should().Be("6925_GB7RDG");
        mail.Subject.Should().Be("Re: Testing WP routing");
        mail.Body.Should().StartWith("What hath god wrought\n\nSent from my iPhone");
        mail.Body.Should().EndWith("***");

        mail.DateTime.Should().BeNull();
        mail.Mid.Should().BeNull();
        mail.ContentTransferEncoding.Should().BeNull();
        mail.ContentType.Should().BeNull();
        mail.Mbo.Should().BeNull();
    }

    [Fact]
    public void ParseMailPage02()
    {
        var html = File.ReadAllText("TestData/mailsample-02.html");
        var result = WebmailParser.Parse(html, Mock.Of<ILogger>());

        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Input.Should().BeNull();

        var mail = result.Value;

        mail.Id.Should().Be(6910);
        mail.Mid.Should().Be("34310_EI2GYB");
        mail.DateTime.Should().Be(new DateTime(2024, 10, 11, 22, 34, 0, DateTimeKind.Utc));
        mail.Type.Should().Be('B');
        mail.From.Should().Be("EI2GYB@EI2GYB.DGL.IRL.EURO");
        mail.To.Should().Be("ASTRO@WW");
        mail.Subject.Should().Be("This Week's Sky at a Glance, October 11 - 20");
        mail.Mbo.Should().Be("GB7BEX");
        mail.ContentType.Should().Be("text/plain");
        mail.ContentTransferEncoding.Should().Be("8bit");

        mail.Date.Should().Be(new MonthAndDay(10, 11));
        mail.Time.Should().Be(new TimeOnly(22, 34));
        mail.Bid.Should().BeNull();

        mail.Body.Should().StartWith("R:241011/2234Z 54227@GB7BEX.#38.GBR.EURO LinBPQ6.0.24\nR:241011/2231Z 34310@EI2GYB.DGL.IRL.EURO LinBPQ6.0.24\n\n");
        mail.Body.Should().EndWith("================================================================================");
    }
}
