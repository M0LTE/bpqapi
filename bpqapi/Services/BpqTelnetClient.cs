using bpqapi.Models;
using bpqapi.Parsers;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace bpqapi.Services;

public class BpqTelnetClient(IOptions<BpqApiOptions> options) : IDisposable
{
    private readonly TcpClient client = new();
    private NetworkStream? stream;
    private StreamReader? reader;
    private StreamWriter? writer;

    public async Task<TelnetLoginResult> Login(string user, string password)
    {
        if (State != BpqSessionState.PreLogin)
        {
            throw new InvalidOperationException("Already logged in");
        }

        if (!options.Value.TelnetTcpPort.HasValue)
        {
            throw new InvalidOperationException("TelnetTcpPort is not configured");
        }

        if (string.IsNullOrWhiteSpace(options.Value.Ctext))
        {
            throw new InvalidOperationException("Ctext is not configured");
        }

        await client.ConnectAsync(options.Value.Uri.Host, options.Value.TelnetTcpPort.Value);
        client.ReceiveTimeout = 5000;
        stream = client.GetStream();
        reader = new(stream);
        writer = new(stream);
        writer.AutoFlush = true;
        writer.NewLine = "\r\n"; // seems even Linux BPQ expects CRLF

        var userPromptResult = reader.Expect("user:");
        if (!userPromptResult.success)
        {
            throw new ProtocolErrorException("Expected 'user:' - is this a BPQ telnet port?");
        }

        writer.WriteLine(user);

        var usernameCorrect = reader.ReadUntil(new Dictionary<string, bool> {
            { "user:", false }, {"password:", true }
        });

        if (!usernameCorrect)
        {
            return TelnetLoginResult.UserInvalid;
        }

        writer.WriteLine(password);

        // a CTEXT line that reads as follows:
        //   CTEXT=Welcome to GB7RDG Telnet Server\n Enter ? for list of commands\n\n
        // comes through like this:
        //   {0d}{0a}Welcome to GB7RDG Telnet Server{0d}{0a} Enter ? for list of commands{0d}{0a}{0d}{0a}{0d}

        // i.e. each "\n" (not newline but the actual characters '\', 'n' in the ctext, which is single-line)
        // is replaced with a CR LF, and a \r is added to the end

        var successMatch = options.Value.Ctext.Replace("\\n", "\r\n") + "\r";

        var result = reader.ReadUntil(new Dictionary<string, TelnetLoginResult> {
            { "password:",  TelnetLoginResult.PasswordInvalid },
            { successMatch, TelnetLoginResult.Success }
        });

        if (result == TelnetLoginResult.Success)
        {
            State = BpqSessionState.LoggedIn;
        }

        return result;
    }

    public Task<List<MailListEntity>> MessageList()
    {
        if (State != BpqSessionState.LoggedIn)
        {
            throw new InvalidOperationException("Not logged in");
        }

        writer!.WriteLine("bbs");

        // ...Last listed is 4029{0d}{0a}de GB7RDG>{0d}{0a}

        var (success, matchingValue) = reader!.Expect(s => s.Contains("\r\nde ") && s.EndsWith(">\r\n"));
        if (!success)
        {
            throw new ProtocolErrorException("Didn't get BBS when expected");
        }

        bbsPrompt = matchingValue.Split("\r\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Last();

        // turn off paging
        writer!.WriteLine("op 0"); 
        reader!.Expect($"Page Length is 0\r\n{bbsPrompt}");

        writer!.WriteLine("la");
        var (ok, response) = reader!.Expect(bbsPrompt);
        
        if (!ok)
        {
            throw new ProtocolErrorException("Didn't get LA response");
        }

        var lines = response.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).SkipLast(1);

        var messages = lines.Select(ParseMailListLine).ToList();

        return Task.FromResult(messages);
    }

    public static MailListEntity ParseMailListLine(string line)
    {
        var id = line[0..6];
        var day = line[7..9];
        var mon = line[10..13];
        var type = line[14];
        var state = line[15];
        var len = line[17..24];
        var from = line[25..31];
        var at = line[32..39];
        var to = line[40..46];
        var subject = line[47..];
        var entity = new MailListEntity
        {
            Id = int.Parse(id),
            Date = new MonthAndDay(months[mon], int.Parse(day)),
            Type = type,
            State = state,
            Length = int.Parse(len),
            From = from.Trim(),
            At = at.StartsWith('@') ? at[1..].Trim() : string.Empty,
            To = to.Trim(),
            Subject = subject.Trim()
        };
        return entity;
    }

    private static readonly Dictionary<string, int> months = new()
    {
        ["Jan"] = 1,
        ["Feb"] = 2,
        ["Mar"] = 3,
        ["Apr"] = 4,
        ["May"] = 5,
        ["Jun"] = 6,
        ["Jul"] = 7,
        ["Aug"] = 8,
        ["Sep"] = 9,
        ["Oct"] = 10,
        ["Nov"] = 11,
        ["Dec"] = 12
    };

    private string? bbsPrompt;

    public void Dispose()
    {
        ((IDisposable)client).Dispose();
    }

    public BpqSessionState State { get; private set; } = BpqSessionState.PreLogin;

    public enum BpqSessionState
    {
        PreLogin, LoggedIn
    }
}

public class ProtocolErrorException(string? message) : Exception(message)
{
    public ProtocolErrorException(string? message, string bufferContents) : this(message)
    {
        BufferContents = bufferContents;
    }

    public string? BufferContents { get; }
}

public enum TelnetLoginResult
{
    UserInvalid, PasswordInvalid, Success
}

internal static class ExtensionMethods
{
    /// <summary>
    /// Read until the underlying client ReadTimeout expires, or the StreamReader ends with the matching string.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="match"></param>
    /// <returns>The matching text if it was found, else null</returns>
    public static (bool success, string matchingValue) Expect(this StreamReader reader, string match)
    {
        return Expect(reader, s => s.EndsWith(match));
    }

    public static (bool success, string matchingValue) Expect(this StreamReader reader, Func<string, bool> predicate)
    {
        var buffer = new List<byte>();

        while (true)
        {
            try
            {
                buffer.Add((byte)reader.Read());
            }
            catch (IOException)
            {
                throw new ProtocolErrorException("Failed to match predicate. Buffer contents: " + buffer.AsString().Printable(), buffer.AsString().Printable());
            }

            //Debug.WriteLine(buffer.AsString());

            var s = Encoding.UTF8.GetString(buffer.ToArray());
            if (predicate(s))
            {
                return (true, s);
            }
        }
    }

    /// <summary>
    /// Read until the underlying client ReadTimeout expires, or one of the matches is found at the end of the StreamReader output.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="matches">A map of strings to look for, and corresponding values for this method to return for each case.</param>
    /// <returns></returns>
    /// <exception cref="IOException">If the system does not return one of the expected strings before the ReadTimeout</exception>
    public static T ReadUntil<T>(this StreamReader reader, Dictionary<string, T> matches)
    {
        var buffer = new List<byte>();

        while (true)
        {
            buffer.Add((byte)reader.Read());
            Debug.WriteLine(buffer.AsString());

            foreach (var match in matches)
            {
                if (buffer.EndsWith(match.Key))
                {
                    return match.Value;
                }
            }
        }
    }

    public static string AsString(this List<byte> list)
    {
        var sb = new StringBuilder();

        foreach (var b in list)
        {
            if (b >= 0x20 && b < 0x7F)
            {
                sb.Append((char)b);
            }
            else
            {
                sb.Append($"{{{b.ToString("X2").ToLower()}}}");
            }
        }

        return sb.ToString();
    }

    public static bool EndsWith(this List<byte> list, string value)
    {
        if (list.Count < value.Length)
        {
            return false;
        }

        for (int i = 0; i < value.Length; i++)
        {
            if (list[list.Count - value.Length + i] != value[i])
            {
                return false;
            }
        }

        Debug.WriteLine($"Matched '{Printable(value)}'");
        return true;
    }

    public static string Printable(this string value)
    {
        var sb = new StringBuilder();

        foreach (char b in value)
        {
            if (b >= 0x20 && b < 0x7F)
            {
                sb.Append(b);
            }
            else
            {
                sb.Append($"{{{((byte)b).ToString("X2").ToLower()}}}");
            }
        }

        return sb.ToString();
    }
}