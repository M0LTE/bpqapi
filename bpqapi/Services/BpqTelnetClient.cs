using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace bpqapi.Services;

public class BpqTelnetClient(IOptions<BpqApiOptions> options)
{
    private readonly TcpClient client = new();

    public async Task<TelnetLoginResult> Login(string user, string password)
    {
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
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream);
        using StreamWriter writer = new(stream);
        writer.AutoFlush = true;
        writer.NewLine = "\r\n"; // seems even Linux BPQ expects CRLF

        if (!reader.Expect("user:"))
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

    public BpqSessionState State { get; private set; } = BpqSessionState.PreLogin;

    public enum BpqSessionState
    {
        PreLogin, LoggedIn
    }
}

public class ProtocolErrorException(string? message) : Exception(message)
{
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
    /// <returns></returns>
    public static bool Expect(this StreamReader reader, string match)
    {
        return ReadUntil(reader, new Dictionary<string, bool> { { match, true } });
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

    private static string Printable(string value)
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