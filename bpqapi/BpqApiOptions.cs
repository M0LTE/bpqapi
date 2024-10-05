namespace bpqapi;

public class BpqApiOptions
{
    public required Uri Uri { get; set; }
    public required string SysopUsername { get; set; }
    public required string SysopPassword { get; set; }
}
