using bpqapi.Models;
using bpqapi.Parsers;
using FluentAssertions;

namespace bpqapi_tests;

public class MailForwardingPageParserTests
{
    private const string getRequestToMailFwd = @"<!-- Version 4 11/11/2019 -->
<!DOCTYPE html>
<style type=""text/css"">
    input.btn:active {
        background: black;
        color: white;
    }

    submit.btn:active {
        background: black;
        color: white;
    }

    #outer {
        width: 900px;
        height: 620px;
        position: absolute;
        border: 0px solid;
        font-family: monospace;
    }

    #main {
        width: 600px;
        position: absolute;
        height: 610px;
        left: 295px;
        border: 2px solid;
    }

    #left {
        position: absolute;
        width: 290px;
        height: 550px;
        border: 0px solid;
        overflow: auto;
    }

    #common {
        position: absolute;
        width: 180px;
        height: 540px;
        border: 2px solid;
        overflow: auto;
    }

    #sidebar {
        position: absolute;
        width: 95px;
        left: 190px;
        height: 540px;
        border: 2px solid;
        overflow: auto;
    }
</style>
<title>Edit Forwarding</title>
<script type=""text/javascript"">
    var Details;
    var Sidebar;
    var fromleft;
    var Outer;
    var Key = ""M0000FAB73A83""
    function initialize() {
        resize();
        Details = document.getElementById(""main"");
        Details.innerHTML = ""waiting for data..."";
        GetData("""");
    }
    function resize() {
        var w = window
          , d = document
          , e = d.documentElement
          , g = d.getElementsByTagName('body')[0];
        x = w.innerWidth || e.clientWidth || g.clientWidth;
        y = w.innerHeight || e.clientHeight || g.clientHeight;
        fromleft = (x / 2) - 450;
        if (fromleft < 0) {
            fromleft = 0;
        }
        Outer = document.getElementById(""outer"");
        Outer.style.left = fromleft + ""px"";
    }
    function save(form) {
        var msg;
        var fn = ""FWDSave?"" + Key;
        msg = form.TO.value + ""|"" + form.AT.value + ""|"" + form.Times.value + ""|"" + form.FWD.value + ""|"" + form.HRB.value + ""|"" + form.HRP.value + ""|"" + form.BBSHA.value + ""|"" + form.EnF.checked + ""|"" + form.Interval.value + ""|"" + form.EnR.checked + ""|"" + form.RInterval.value + ""|"" + form.NoWait.checked + ""|"" + form.Blocked.checked + ""|"" + form.FBBBlock.value + ""|"" + form.Personal.checked + ""|"" + form.Bin.checked + ""|"" + form.B1.checked + ""|"" + form.B2.checked + ""|"" + form.CTRLZ.checked + ""|"" + form.ConTimeOut.value + ""|"";
        post(fn, msg);
    }
    function startf(form) {
        var fn = ""FWDSave?"" + Key;
        post(fn, ""StartForward"", """");
    }
    function copyf(form) {
        var msg;
        var fn = ""FWDSave?"" + Key;
        msg = ""CopyForward|"" + form.CopyCall.value;
        post(fn, msg);
    }
    function post(url, data) {
        if (window.XMLHttpRequest) {
            request = new XMLHttpRequest();
        } else {
            request = new ActiveXObject(""Microsoft.XMLHTTP"");
        }
        request.onreadystatechange = function() {
            if (request.readyState == 4) {
                Details.innerHTML = request.responseText;
            }
        }
        request.open(""POST"", url, true);
        request.send(data);
    }
    function GetDetails(call) {
        var fn = ""FwdDetails?"" + Key;
        post(fn, call);
    }
    function GetData() {
        if (window.XMLHttpRequest) {
            request = new XMLHttpRequest();
        } else {
            request = new ActiveXObject(""Microsoft.XMLHTTP"");
        }
        request.onreadystatechange = function() {
            if (request.readyState == 4) {
                var text = request.responseText;
                var lines = text.split(""|"");
                var i = 0;
                var infoArea = document.getElementById(""sidebar"");
                var strInfo = '<table>';
                while (i < lines.length - 1) {
                    var clink = '<a href=javascript:GetDetails(""' + lines[i] + '"")>' + lines[i] + '</a>';
                    var tableRow = '<tr><td>' + clink + '</td></tr>';
                    strInfo += tableRow;
                    i = i + 1;
                }
                infoArea.innerHTML = strInfo + ""</table>"";
                GetDetails(lines[0]);
            }
        }
        request.open(""POST"", ""FwdList.txt?M0000FAB73A83"", true);
        request.send();
    }
</script>
</head>
<body onload=initialize() onresize=resize() style=""background-image: url(/background.jpg);"">
    <h3 align=center>BPQ32 BBS GB7RDG</h3>
    <table align=center bgcolor=white border=1 cellpadding=2>
        <tbody>
            <tr>
                <td>
                    <a href=""/Mail/Status?M0000FAB73A83"">Status</a>
                </td>
                <td>
                    <a href=""/Mail/Conf?M0000FAB73A83"">Configuration</a>
                </td>
                <td>
                    <a href=""/Mail/Users?M0000FAB73A83"">Users</a>
                </td>
                <td>
                    <a href=""/Mail/Msgs?M0000FAB73A83"">Messages</a>
                </td>
                <td>
                    <a href=""/Mail/FWD?M0000FAB73A83"">Forwarding</a>
                </td>
                <td>
                    <a href=""/Mail/Wel?M0000FAB73A83"">Welcome Msgs &amp;Prompts</a>
                </td>
                <td>
                    <a href=""/Mail/HK?M0000FAB73A83"">Housekeeping</a>
                </td>
                <td>
                    <a href=""/Mail/WP?M0000FAB73A83"">WP Update</a>
                </td>
                <td>
                    <a href=/Webmail>WebMail</a>
                </td>
                <td>
                    <a href=""/"">Node Menu</a>
                </td>
            </tr>
        </tbody>
    </table>
    <br/>
    <div id=""outer"">
        <div id=""left"">
            <div id=""common"">
                <form style=""font-family: monospace; text-align: center;"" method=post action=/Mail/FwdCommon?M0000FAB73A83>
                    Max size to Send <input value=30000 size=3 name=MaxTX>
                    <br>
                    <br>
                    Max Size to Receive <input value=30001 size=3 name=MaxRX>
                    <br>
                    <br>
                    Max age for Bulls <input value=60 size=3 name=MaxAge>
                    <br>
                    <br>
                    Warn if no route<br>
                    &nbsp;for P or T <input checked=checked name=WarnNoRoute type=checkbox/>
                    <br/>
                    <br/>
                    Use Local Time &nbsp;<input name=LocalTime type=checkbox/>
                    <br/>
                    <br/>
                    Send P Msgs to more than one BBS &nbsp;<input checked=checked name=SendPtoMultiple type=checkbox/>
                    <br/>
                    <br/>
                    Use 4 Char Continent Codes &nbsp;<input name=FourCharCont type=checkbox/>
                    <br/>
                    <br/>
                    Aliases<br/>
                    <br/>
                    <textarea rows=8 cols=12 name=Aliases>TEST
LINE 2
</textarea>
                    <br>
                    <br>
                    <input name=Submit value=Update type=submit class='btn'>
                </form>
            </div>
            <div id=""sidebar""></div>
        </div>
        <div id=""main""></div>
    </div>
</body>
</html></body></html>
";

	[Fact]
    public void TestPageParsing()
    {
		var options = MailForwardingPageParser.ParseOptions(getRequestToMailFwd);
        options.MaxSizeToSend.Should().Be(30000);
        options.MaxSizeToReceive.Should().Be(30001);
        options.MaxAgeForBulls.Should().Be(TimeSpan.FromDays(60));
        options.WarnIfNoRouteForPOrT.Should().BeTrue();
        options.UseLocalTime.Should().BeFalse();
        options.SendPMessagesToMoreThanOneBbs.Should().BeTrue();
        options.Use4CharContinentCodes.Should().BeFalse();
        options.Aliases.Should().BeEquivalentTo("TEST", "LINE 2");
    }

    private const string postRequestToFwdList = @"GB7BDH|GB7BEX|GB7BPQ|GB7BRK|GB7BSK|GB7CIP|GB7HIB|GB7IOW|GB7LAN|GB7LOX|GB7OXF|GB7RDG|M5MPC|</body></html>";

    [Fact]
    public void TestForwardingListParsing()
    {
        var partners = MailForwardingPageParser.ParsePartnerList(postRequestToFwdList);
        partners.Should().BeEquivalentTo("GB7BDH|GB7BEX|GB7BPQ|GB7BRK|GB7BSK|GB7CIP|GB7HIB|GB7IOW|GB7LAN|GB7LOX|GB7OXF|GB7RDG|M5MPC".Split('|'));
    }

    private const string postRequestToFwdDetails = @"<!-- Version 3 11/11/2019 -->
<h3 align=""center"">Forwarding Config for GB7BDH - 0 Messages Queued</h3><form style=""font-family: monospace; text-align: center"" method=post action=/Mail/FWD?M0000FAB73A83>
&nbsp;&nbsp;TO &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AT&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;TIMES&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; Connect Script<br>
<textarea wrap=hard rows=8 cols=10 name=TO></textarea> 
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
        var station = MailForwardingPageParser.ParseStationResponse(postRequestToFwdDetails);
        station.QueueLength.Should().Be(0);
        station.To.Should().BeEmpty();
        station.At.Should().BeEquivalentTo("OARC", "GBR", "WW");
        station.Times.Should().BeEmpty();
        station.ConnectScript.Should().BeEquivalentTo("PAUSE 2", "INTERLOCK 3", "NC 3 !M1BFP-1", "C 2 GB7BDH-1");
        station.HierarchicalRoutes.Should().BeEmpty();
        station.HR.Should().BeEquivalentTo("#49.GBR.EURO", "#53.GBR.EURO");
        station.BbsHa.Should().BeEmpty();
        station.EnableForwarding.Should().BeFalse();
        station.ForwardingInterval.Should().Be(TimeSpan.FromSeconds(3500));
        station.RequestReverse.Should().BeFalse();
        station.ReverseInterval.Should().Be(TimeSpan.FromSeconds(3600));
        station.SendNewMessagesWithoutWaiting.Should().BeTrue();
        station.FbbBlocked.Should().BeTrue();
        station.MaxBlock.Should().Be(1000);
        station.SendPersonalMailOnly.Should().BeFalse();
        station.AllowBinary.Should().BeTrue();
        station.UseB1Protocol.Should().BeTrue();
        station.UseB2Protocol.Should().BeFalse();
        station.SendCtrlZInsteadOfEx.Should().BeFalse();
        station.IncomingConnectTimeout.Should().Be(TimeSpan.FromSeconds(120));
    }
}