
using bpqapi.Models;
using System.Text.Json;

namespace bpqapi.Services;

public class BpqMailCache (ILogger<BpqMailCache> logger)
{
    public bool TryGetValue(int id, out MailEntity? cached)
    {
        if (!Directory.Exists("mail"))
        {
            cached = null;
            return false;
        }

        var filename = $"mail/{id}.json";

        if (!File.Exists(filename))
        {
            cached = null;
            return false;
        }

        try
        {
            var json = File.ReadAllText(filename);
            cached = JsonSerializer.Deserialize<MailEntity>(json, options);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read mail cache file {Filename}", filename);
            cached = null;
            return false;
        }
    }

    private static readonly JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    
    internal void Add(int id, MailEntity mail)
    {
        if (!Directory.Exists("mail"))
        {
            Directory.CreateDirectory("mail");
        }

        var json = JsonSerializer.Serialize(mail, options);
        File.WriteAllText($"mail/{id}.json", json);
    }
}