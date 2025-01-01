using bpqapi.Models;
using System.Text.Json;

namespace bpqapi.Services;

public class BpqMailCache(ILogger<BpqMailCache> logger)
{
    private const string cacheDir = "/tmp/mail";

    public bool TryGetValue(int id, out MailEntity? cached)
    {
        if (!Directory.Exists(cacheDir))
        {
            cached = null;
            return false;
        }

        var filename = $"{cacheDir}/{id}.json";

        if (!File.Exists(filename))
        {
            logger.LogInformation("Cache miss for mail {Id}", id);
            cached = null;
            return false;
        }

        try
        {
            var json = File.ReadAllText(filename);
            cached = JsonSerializer.Deserialize<MailEntity>(json, options);
            logger.LogInformation("Cache hit for mail {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cache read error for mail {Id}", id);
            cached = null;
            return false;
        }
    }

    private static readonly JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    internal void Add(int id, MailEntity mail)
    {
        try
        {
            CreateDirectory();
            var json = JsonSerializer.Serialize(mail, options);
            var fn = $"{cacheDir}/{id}.json";
            File.WriteAllText(fn, json);
            logger.LogInformation("Wrote mail cache file {Filename}", fn);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to write mail cache file {Filename}: {message}", id, ex.Message);
        }
    }

    private void CreateDirectory()
    {
        if (!Directory.Exists(cacheDir))
        {
            try
            {
                Directory.CreateDirectory(cacheDir);
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to create mail cache directory {Directory}: {message}", cacheDir, ex.Message);
                return;
            }
        }
    }
}
