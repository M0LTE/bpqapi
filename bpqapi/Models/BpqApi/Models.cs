using System.Text.Json.Serialization;

namespace bpqapi.Models.BpqApi;

public readonly record struct GetLinksResponse
{
    public Link[] Links { get; init; }

    public readonly record struct Link
    {
        public string FarCall { get; init; }
        public string OurCall { get; init; }
        public string Port { get; init; }
        public string State { get; init; }
        public string LinkType { get; init; }
        public string Ax25Version { get; init; }
    }
}

public readonly record struct GetInfoResponse
{
    public BpqInfo Info { get; init; }

    public readonly record struct BpqInfo
    {
        public string NodeCall { get; init; }
        public string Alias { get; init; }
        public string Locator { get; init; }
        public string Version { get; init; }
    }
}

public readonly record struct GetUsersResponse
{
    public User[] Users { get; init; }

    public readonly record struct User
    {
        public string Call { get; init; }
    }
}

public readonly record struct GetNodesResponse
{
    public Node[] Nodes { get; init; }

    public readonly record struct Node
    {
        public string Call { get; init; }
        public string Alias { get; init; }
        public BpqRoute[] Routes { get; init; }

        public readonly record struct BpqRoute
        {
            public string Call { get; init; }
            public int Port { get; init; }
            public int Quality { get; init; }
        }
    }
}

public readonly record struct GetPortsResponse
{
    public Port[] Ports { get; init; }
    public readonly record struct Port
    {
        public string Id { get; init; }
        public string Driver { get; init; }
        public int Number { get; init; }
        public string State { get; init; }
    }
}

public readonly record struct BpqApiMheardElement
{
    [JsonPropertyName("callSign")]
    public string Callsign { get; init; }

    [JsonPropertyName("port")]
    public string Port { get; init; }

    [JsonPropertyName("packets")]
    public int Packets { get; init; }

    [JsonPropertyName("lastHeard")]
    public string LastHeard { get; init; }
}

public readonly record struct BpqGetAccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; }
}