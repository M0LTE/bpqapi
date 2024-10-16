namespace bpqapi;

public class BpqApiOptions
{
    public required Uri Uri { get; set; }
    public int? TelnetTcpPort { get; set; }
    public string? Ctext { get; set; }
}
