using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrescriptionService.Data;
using PrescriptionService.Services;
using Quartz;
using PrescriptionService.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PrescriptionDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
});

builder.Services.AddScoped<IPrescriptionService, PrescriptionService.Services.PrescriptionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Configure Quartz
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("NotificationJob");
    
    q.AddJob<NotificationJob>(opts => opts.WithIdentity(jobKey));
    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("NotificationJob-trigger")
        .WithCronSchedule("0 0 1 * * ?")); // Run at 1:00 AM every day
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<PrescriptionDbContext>();
        
        // Wait for SQL Server to be ready
        var maxRetries = 10;
        var retryCount = 0;
        var delay = TimeSpan.FromSeconds(5);
        
        while (retryCount < maxRetries)
        {
            try
            {
                if (!context.Database.CanConnect())
                {
                    logger.LogInformation($"Attempt {retryCount + 1} of {maxRetries}. Waiting for SQL Server to be ready...");
                    await Task.Delay(delay);
                    retryCount++;
                    continue;
                }
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Attempt {retryCount + 1} of {maxRetries} failed. Retrying in {delay.TotalSeconds} seconds...");
                await Task.Delay(delay);
                retryCount++;
                if (retryCount == maxRetries) throw;
            }
        }

        // Only apply migrations, skip database creation
        if (context.Database.CanConnect())
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

app.Run();
