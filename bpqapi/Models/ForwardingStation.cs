using System.Text.Json.Serialization;

namespace bpqapi.Models;

public record ForwardingStation
{
    [JsonIgnore]
    public string Callsign { get; set; } = "";

    public int QueueLength { get; set; }

    public ForwardingConfig ForwardingConfig { get; set; } = new();
}

public record ForwardingConfig
{
    public string[] To { get; set; } = [];
    public string[] At { get; set; } = [];
    public string[] Times { get; set; } = [];
    public string[] ConnectScript { get; set; } = [];
    public string[] HierarchicalRoutes { get; set; } = [];
    public string[] HR { get; set; } = [];
    public string BbsHa { get; set; } = "";
    public bool EnableForwarding { get; set; }
    public TimeSpan ForwardingInterval { get; set; }
    public bool RequestReverse { get; set; }
    public TimeSpan ReverseInterval { get; set; }
    public bool SendNewMessagesWithoutWaiting { get; set; }
    public bool FbbBlocked { get; set; }
    public int MaxBlock { get; set; }
    public bool SendPersonalMailOnly { get; set; }
    public bool AllowBinary { get; set; }
    public bool UseB1Protocol { get; set; }
    public bool UseB2Protocol { get; set; }
    public bool SendCtrlZInsteadOfEx { get; set; }
    public TimeSpan IncomingConnectTimeout { get; set; }
}