using bpqapi.Models;
using bpqapi.Parsers;
using FluentAssertions;

namespace bpqapi_tests;

public class MailForwardingPageParserTests
{
    [Fact]
    public void TestPageParsing()
    {
        var html = File.ReadAllText("testdata/mailforwardingpage.html");
        var result = MailForwardingPageParser.ParseOptions(html);
        result.Success.Should().BeTrue();
        result.Input.Should().BeNullOrWhiteSpace();
        var options = result.Value;
        options.MaxSizeToSend.Should().Be(30000);
        options.MaxSizeToReceive.Should().Be(30001);
        options.MaxAgeForBulls.Should().Be(TimeSpan.FromDays(60));
        options.WarnIfNoRouteForPOrT.Should().BeTrue();
        options.UseLocalTime.Should().BeFalse();
        options.SendPMessagesToMoreThanOneBbs.Should().BeTrue();
        options.Use4CharContinentCodes.Should().BeFalse();
        options.Aliases.Should().BeEquivalentTo("TEST", "LINE 2");
    }

    [Fact]
    public void TestForwardingListParsing()
    {
        const string apiResponse = @"GB7BDH|GB7BEX|GB7BPQ|GB7BRK|GB7BSK|GB7CIP|GB7HIB|GB7IOW|GB7LAN|GB7LOX|GB7OXF|GB7RDG|M5MPC|</body></html>";
        var result = MailForwardingPageParser.ParsePartnerList(apiResponse);
        result.Success.Should().BeTrue();
        result.Input.Should().BeNullOrWhiteSpace();
        var partners = result.Value;
        partners.Should().BeEquivalentTo("GB7BDH|GB7BEX|GB7BPQ|GB7BRK|GB7BSK|GB7CIP|GB7HIB|GB7IOW|GB7LAN|GB7LOX|GB7OXF|GB7RDG|M5MPC".Split('|'));
    }

    private const string postRequestToFwdDetails = @"<!-- Version 3 11/11/2019 -->
<h3 align=""center"">Forwarding Config for GB7BDH - 5 Messages Queued</h3><form style=""font-family: monospace; text-align: center"" method=post action=/Mail/FWD?M0000FAB73A83>
&nbsp;&nbsp;TO &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AT&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;TIMES&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; Connect Script<br>
<textarea wrap=hard rows=8 cols=10 name=TO>line1 </textarea> 
<textarea wrap=hard rows=8 cols=10 name=AT>OARC
GBR
WW
</textarea> 
<textarea wrap=hard rows=8 cols=10 name=Times></textarea> 
<textarea wrap=hard rows=8 cols=20 name=FWD>PAUSE 2
INTERLOCK 3
NC 3 !M1BFP-1
C 2 GB7BDH-1
</textarea><br>
Hierarchical Routes (Flood Bulls) HR (Personals and Directed Bulls)
<textarea wrap=hard rows=8 cols=30 name=HRB></textarea> 
<textarea wrap=hard rows=8 cols=30 name=HRP>#49.GBR.EURO
#53.GBR.EURO
</textarea>
<br><br>
BBS HA <input value="""" size=50 name=BBSHA> <br>
Enable Forwarding&nbsp;<input name=EnF type=checkbox> Interval <input value=3500 size=3 name=Interval>(Secs)<br>
Request Reverse&nbsp;&nbsp;&nbsp;<input name=EnR type=checkbox> Interval <input value=3600 size=3 name=RInterval>(Secs)<br>
Send new messages without waiting for poll timer<input checked=checked name=NoWait type=checkbox><br>
FBB Blocked <input checked=checked name=Blocked type=checkbox>Max Block <input value=1000 size=3 name=FBBBlock> 
Send Personal Mail Only <input name=Personal type=checkbox><br>
Allow Binary <input checked=checked name=Bin type=checkbox> Use B1 Protocol <input checked=checked name=B1 type=checkbox>&nbsp; Use B2 Protocol<input name=B2 type=checkbox><br>
Send ctrl/Z instead of /ex in text mode forwarding <input name=CTRLZ type=checkbox><br>
Incoming Connect Timeout <input value=120 size=3 name=ConTimeOut>(Secs)<br>
<br>
<input onclick=copyf(this.form) value=""Copy from BBS"" type=button class='btn'>
<input value="""" size=3 name=CopyCall> 
<input onclick=save(this.form) value=Update type=button class='btn'>
<input onclick=startf(this.form) value=""Start Forwarding"" type=button class='btn'>
</form>
</body></html>";

    [Fact]
    public void TestForwardingPartnerResponseParsing()
    {
        var result = MailForwardingPageParser.ParseStationResponse(postRequestToFwdDetails);
        result.Success.Should().BeTrue();
        result.Input.Should().BeNullOrWhiteSpace();
        var station = result.Value;
        station.Callsign.Should().Be("GB7BDH");
        station.QueueLength.Should().Be(5);
        var settings = station.ForwardingConfig;
        settings.To.Should().BeEquivalentTo("line1");
        settings.At.Should().BeEquivalentTo("OARC", "GBR", "WW");
        settings.Times.Should().BeEmpty();
        settings.ConnectScript.Should().BeEquivalentTo("PAUSE 2", "INTERLOCK 3", "NC 3 !M1BFP-1", "C 2 GB7BDH-1");
        settings.HierarchicalRoutes.Should().BeEmpty();
        settings.HR.Should().BeEquivalentTo("#49.GBR.EURO", "#53.GBR.EURO");
        settings.BbsHa.Should().BeEmpty();
        settings.EnableForwarding.Should().BeFalse();
        settings.ForwardingInterval.Should().Be(TimeSpan.FromSeconds(3500));
        settings.RequestReverse.Should().BeFalse();
        settings.ReverseInterval.Should().Be(TimeSpan.FromSeconds(3600));
        settings.SendNewMessagesWithoutWaiting.Should().BeTrue();
        settings.FbbBlocked.Should().BeTrue();
        settings.MaxBlock.Should().Be(1000);
        settings.SendPersonalMailOnly.Should().BeFalse();
        settings.AllowBinary.Should().BeTrue();
        settings.UseB1Protocol.Should().BeTrue();
        settings.UseB2Protocol.Should().BeFalse();
        settings.SendCtrlZInsteadOfEx.Should().BeFalse();
        settings.IncomingConnectTimeout.Should().Be(TimeSpan.FromSeconds(120));
    }
}