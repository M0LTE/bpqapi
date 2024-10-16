using bpqapi.Services;
using FluentAssertions;

namespace bpqapi_tests;

[Trait("Category", "Integration")]
public class BpqTelnetTests
{
    private static Options options = new(new Uri("http://gb7rdg-node:8008"), 8010, ctext: "Welcome to GB7RDG Telnet Server\\n Enter ? for list of commands\\n\\n");
    private readonly BpqTelnetClient target = new(options);
    private const string password = "xx"; // don't commit this

    [Fact]
    public async Task TestBadUsername() => (await target.Login("aa", "irrelevant")).Should().Be(TelnetLoginResult.UserInvalid);

    [Fact]
    public async Task TestBadPassword() => (await target.Login("tf", "wrongpass")).Should().Be(TelnetLoginResult.PasswordInvalid);

    [Fact]
    public async Task TestOkPassword() => (await target.Login("tf", password)).Should().Be(TelnetLoginResult.Success); // will fail because the password is local to me

    [Fact]
    public async Task TestBbs()
    {
        var result = await target.Login("tf", password);
        result.Should().Be(TelnetLoginResult.Success);
        var messages = await target.MessageList();
        messages.Should().NotBeEmpty();
        messages.All(m => m.Id > 0).Should().BeTrue();
    }

    [Fact]
    public void TestTelnetMailParser()
    {
        var line = "409699 12-Jan PN 1234217 EI5IYB @GB7RDG M10NVK Hello from Ireland";
        var result = BpqTelnetClient.ParseMailListLine(line);

        result.Id.Should().Be(409699);
        result.Date.Day.Should().Be(12);
        result.Date.Month.Should().Be(1);
        result.Type.Should().Be('P');
        result.State.Should().Be('N');
        result.Length.Should().Be(1234217);
        result.From.Should().Be("EI5IYB");
        result.At.Should().Be("GB7RDG");
        result.To.Should().Be("M10NVK");
        result.Subject.Should().Be("Hello from Ireland");
    }

    [Fact]
    public void TestSparseTelnetMailParser()
    {
        var line = "4       2-Jan PN       7 M5ET           NS1A   ";
        var result = BpqTelnetClient.ParseMailListLine(line);

        result.Id.Should().Be(4);
        result.Date.Day.Should().Be(2);
        result.Date.Month.Should().Be(1);
        result.Type.Should().Be('P');
        result.State.Should().Be('N');
        result.Length.Should().Be(7);
        result.From.Should().Be("M5ET");
        result.At.Should().Be("");
        result.To.Should().Be("NS1A");
        result.Subject.Should().Be("");
    }
}
