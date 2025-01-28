using System.Text;
using AuthService.Data;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create database and apply migrations
var maxRetries = 5;
var retryCount = 0;
var delay = TimeSpan.FromSeconds(5);

while (retryCount < maxRetries)
{
    try 
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            db.Database.Migrate();
            Console.WriteLine("Database migrated successfully");
            break;
        }
    }
    catch (Exception ex)
    {
        retryCount++;
        Console.WriteLine($"Attempt {retryCount} of {maxRetries}. An error occurred while connecting to the database: {ex.Message}");
        
        if (retryCount < maxRetries)
        {
            Console.WriteLine($"Waiting {delay.TotalSeconds} seconds before retrying...");
            Thread.Sleep(delay);
        }
        else
        {
            Console.WriteLine("Could not connect to the database after multiple attempts. Exiting...");
            throw;
        }
    }
}

app.Run();
