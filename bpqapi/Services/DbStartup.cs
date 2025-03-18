using bpqapi.Models;
using SQLite;

namespace bpqapi.Services;

public static class DbInfo
{
    private static readonly SQLiteConnectionString connectionString;

    private static readonly string workingDirectory = Directory.GetCurrentDirectory();
    private static readonly string dataDirectory = Path.Combine(workingDirectory, "data");
    private static readonly string dbFilename = "bpqapi.db";
    private static readonly string dbPath = Path.Combine(dataDirectory, dbFilename);

    static DbInfo()
    {

        try
        {
            var file = Path.Combine(dataDirectory, "deleteme");
            File.WriteAllText(file, "hello world");
            File.Delete(file);
            Console.WriteLine($"DB path {dataDirectory} is writeable");
            connectionString = new SQLiteConnectionString(dbPath);
        }
        catch (Exception)
        {
            Console.WriteLine($"Error writing to /app/data- using shared in-memory database. Provide a writeable folder at {dataDirectory} to persist the db");
            connectionString = new SQLiteConnectionString("file::memory:?cache=shared");
        }
    }

    public static SQLiteConnection GetConnection() => new(connectionString);

    public static SQLiteAsyncConnection GetAsyncConnection() => new(connectionString);
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