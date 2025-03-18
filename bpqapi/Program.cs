using bpqapi;
using bpqapi.Controllers;
using bpqapi.Services;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

/*Console.WriteLine("Working directory: " + Directory.GetCurrentDirectory());
TryWrite("/app/data");

static void TryWrite(string directory)
{
    var file = Path.Combine(directory, "deleteme");
    Console.WriteLine("Trying to write to " + file);

    try
    {
        File.WriteAllText(file, "hello world");
        Console.WriteLine("Success writing to " + file);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error writing to {file}: {ex.Message}");
    }
}

return;*/

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddHostedService<DbStartup>();
    builder.Services.AddHostedService<BpqConnectivityCheck>();
    builder.Services.AddHostedService<ConfigCheckService>();
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "bpqapi.xml");
        c.IncludeXmlComments(filePath);
    });
    builder.Services.Configure<BpqApiOptions>(builder.Configuration.GetSection("bpq"));
    builder.Services.AddSingleton<BpqUiService>();
    builder.Services.AddSingleton<BpqNativeApiService>();
    builder.Services.AddSingleton<MailService>();
    builder.Services.AddSingleton<MailRepository>();
    builder.Services.AddTransient<BpqTelnetClient>();
    builder.Services.AddHttpClient<BpqUiService>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(5);
    }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip });

    builder.Services.AddHttpClient<BpqNativeApiService>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(5);
    }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip });


    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "BPQ API", Version = "v1" });
        c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
        {
            Name = "Basic authorisation",
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
            In = ParameterLocation.Header,
            Description = "Basic Authorization header using the Bearer scheme."
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "basic" } }, [] } });
    });

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

    builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();
    app.UseSwaggerUI();
    //}

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}