using bpqapi.Models;
using SQLite;

namespace bpqapi.Services;

public static class DbInfo
{
    private static string GetPath()
    {
        if (Directory.Exists("data"))
        {
            return "data/bpqapi.db";
        }

        return "bpqapi.db";
    }

    public static SQLiteConnection GetConnection() => new(GetPath());

    public static SQLiteAsyncConnection GetAsyncConnection() => new(GetPath());
}

public class DbStartup(ILogger<DbStartup> logger) : IHostedService
{
    private readonly SQLiteConnection db = DbInfo.GetConnection();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"DB: {db.DatabasePath}");

        db.CreateTable<DbMail>();

        logger.LogInformation("DB schema refreshed");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}