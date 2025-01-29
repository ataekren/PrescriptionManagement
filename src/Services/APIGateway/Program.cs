using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add health checks
builder.Services.AddHealthChecks();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add Ocelot configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration)
    .AddPolly();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add health check endpoint
app.MapHealthChecks("/health");

// Use CORS before routing
app.UseCors("AllowAll");

// Add error handling
app.UseExceptionHandler("/Error");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Configure Ocelot middleware with error handling
await app.UseOcelot();

app.Run("http://localhost:5000");

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
