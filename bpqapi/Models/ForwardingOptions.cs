namespace bpqapi.Models;

public record ForwardingOptions
{
    public int MaxSizeToSend { get; set; }
    public int MaxSizeToReceive { get; set; }
    public TimeSpan MaxAgeForBulls { get; set; }
    public bool WarnIfNoRouteForPOrT { get; set; }
    public bool UseLocalTime { get; set; }
    public bool SendPMessagesToMoreThanOneBbs { get; set; }
    public bool Use4CharContinentCodes { get; set; }
    public string[] Aliases { get; set; } = [];
}

/*public record 
    public string[] Stations { get; set; } = [];
    public ForwardingStation SelectedStation { get; set; } = new ForwardingStation();
}*/
