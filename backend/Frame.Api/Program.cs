using Frame.Api.Extensions;
using Frame.Api.Middleware;
using Frame.Application;
using Frame.Infrastructure;
using Serilog;
using Serilog.Events;

// A minimal logger for startup, so even a crash before the app is built gets logged.
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var logsPath = Path.Combine(builder.Environment.ContentRootPath, "logs", "frame-.log");

    // ===== Logging: console + one file per day, kept 14 days =====
    builder.Services.AddSerilog((services, logger) => logger
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: logsPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{RequestId}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));

    // ===== Services: one line per layer =====
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddFrameApi(builder.Configuration);

    var app = builder.Build();

    // ===== Startup: migrate the database and seed studios + admin =====
    await app.Services.InitializeDatabaseAsync();

    // ===== HTTP pipeline (order matters) =====
    app.UseSerilogRequestLogging();             // 1. outermost: logs the FINAL status (409, not a fake 500)
    app.UseMiddleware<ExceptionMiddleware>();   // 2. turns every exception below into a JSON response

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();                              // 3. which origins may call
    app.UseRateLimiter();                       // 4. login attempt limits
    app.UseAuthentication();                    // 5. who is calling (reads the JWT)
    app.UseAuthorization();                     // 6. are they allowed (policies)
    app.MapControllers();                       // 7. the endpoints

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    // HostAbortedException is normal when "dotnet ef" builds the app: not a crash.
    Log.Fatal(ex, "Frame API failed to start");
}
finally
{
    await Log.CloseAndFlushAsync();
}