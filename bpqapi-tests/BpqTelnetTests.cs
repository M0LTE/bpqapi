using bpqapi.Services;
using FluentAssertions;

namespace bpqapi_tests;

public class BpqTelnetTests
{
    private Options options => new(new Uri("http://gb7rdg-node:8008"), 8010, ctext: "Welcome to GB7RDG Telnet Server\\n Enter ? for list of commands\\n\\n");
    private BpqTelnetClient target => new(options);

    [Fact]
    public async Task TestBadUsername() => (await target.Login("aa", "irrelevant")).Should().Be(TelnetLoginResult.UserInvalid);

    [Fact]
    public async Task TestBadPassword() => (await target.Login("tf", "wrongpass")).Should().Be(TelnetLoginResult.PasswordInvalid);

    [Fact]
    public async Task TestOkPassword() => (await target.Login("tf", "xx")).Should().Be(TelnetLoginResult.Success); // will fail because the password is local to me
}
