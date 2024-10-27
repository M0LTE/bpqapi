using bpqapi.Parsers;
using FluentAssertions;

namespace bpqapi_tests;

public class WebmailResponseParserTests
{
    [Fact]
    public void TestPageParsing()
    {
        var html = File.ReadAllText("testdata/webmaillistingpage-01.html");
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
        mail[0].Type.Should().Be('B');
        mail[0].State.Should().Be('N');
        mail[0].Length.Should().Be(26386);
        mail[0].Subject.Should().Be("October 2024 NTS Letter");

        mail.Should().HaveCount(221);

        mail.Last().Id.Should().Be(1111);
        mail.Last().To.Should().Be("NTSxxx");
        mail.Last().At.Should().Be("ARRLxx");
        mail.Last().From.Should().Be("CX2SAx");
    }

    [Fact]
    public void TestPageParsing02()
    {
        var html = File.ReadAllText("testdata/webmaillistingpage-02.html");
        var result = WebmailListingParser.Parse(html);
        result.Success.Should().BeTrue();
        result.Input.Should().BeNullOrWhiteSpace();

        var (token, mail) = result.Value;

        mail[0].Id.Should().Be(54273);
        mail[0].To.Should().Be("G7TAJ");
        mail[0].At.Should().Be("");
        mail[0].From.Should().Be("SYSTEM");
        mail[0].Date.Month.Should().Be(10);
        mail[0].Date.Day.Should().Be(13);
        mail[0].Type.Should().Be('P');
        mail[0].State.Should().Be('N');
        mail[0].Length.Should().Be(61);
        mail[0].Subject.Should().Be("New User G8PZT");

        mail.Should().HaveCount(2126);

        mail.Last().Id.Should().Be(51854);
        mail.Last().To.Should().Be("DX");
        mail.Last().At.Should().Be("WW");
        mail.Last().From.Should().Be("LU9DCE");
    }

    [Fact]
    public void ParseWebmailLineTests01()
    {
        var line = " 13-Jun BF 10946 DX      WW      LU9DCE  DX OPERATION 13-JUN24";
        var mail = WebmailListingParser.ParseMailItem(line, 1);
        mail.Date.Day.Should().Be(13);
        mail.Date.Month.Should().Be(6);
        mail.Type.Should().Be('B');
        mail.State.Should().Be('F');
        mail.Length.Should().Be(10946);
        mail.To.Should().Be("DX");
        mail.At.Should().Be("WW");
        mail.From.Should().Be("LU9DCE");
        mail.Subject.Should().Be("DX OPERATION 13-JUN24");
    }

    [Fact]
    public void ParseWebmailLineTests02()
    {
        var line = " 03-Jun BF  3731 KEP     WW      NC8Q    ";
        var mail = WebmailListingParser.ParseMailItem(line, 1);
        mail.Date.Day.Should().Be(3);
        mail.Date.Month.Should().Be(6);
        mail.Type.Should().Be('B');
        mail.State.Should().Be('F');
        mail.Length.Should().Be(3731);
        mail.To.Should().Be("KEP");
        mail.At.Should().Be("WW");
        mail.From.Should().Be("NC8Q");
        mail.Subject.Should().BeEmpty();
    }

    [Fact]
    public void ParseWebmailLineTests03()
    {
        var line = " 13-Jun BF 10946 DX              LU9DCE  DX OPERATION 13-JUN24";
        var mail = WebmailListingParser.ParseMailItem(line, 1);
        mail.Date.Day.Should().Be(13);
        mail.Date.Month.Should().Be(6);
        mail.Type.Should().Be('B');
        mail.State.Should().Be('F');
        mail.Length.Should().Be(10946);
        mail.To.Should().Be("DX");
        mail.At.Should().Be("");
        mail.From.Should().Be("LU9DCE");
        mail.Subject.Should().Be("DX OPERATION 13-JUN24");
    }

    [Fact]
    public void ParseWebmailLineTests04()
    {
        var line = " 13-Jun BF  1946 DX              LU9DCE  DX OPERATION 13-JUN24";
        var mail = WebmailListingParser.ParseMailItem(line, 1);
        mail.Date.Day.Should().Be(13);
        mail.Date.Month.Should().Be(6);
        mail.Type.Should().Be('B');
        mail.State.Should().Be('F');
        mail.Length.Should().Be(1946);
        mail.To.Should().Be("DX");
        mail.At.Should().Be("");
        mail.From.Should().Be("LU9DCE");
        mail.Subject.Should().Be("DX OPERATION 13-JUN24");
    }

    [Fact]
    public void ParseWebmailLineTests05()
    {
        var line = " 13-Jun BF 1946 DX              LU9DCE  DX OPERATION 13-JUN24";
        var mail = WebmailListingParser.ParseMailItem(line, 1);
        mail.Date.Day.Should().Be(13);
        mail.Date.Month.Should().Be(6);
        mail.Type.Should().Be('B');
        mail.State.Should().Be('F');
        mail.Length.Should().Be(1946);
        mail.To.Should().Be("DX");
        mail.At.Should().Be("");
        mail.From.Should().Be("LU9DCE");
        mail.Subject.Should().Be("DX OPERATION 13-JUN24");
    }
}
