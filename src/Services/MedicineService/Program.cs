using MedicineService.Repositories;
using MedicineService.Services;
using MedicineService.Settings;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddHttpClient();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();

// Add Redis caching with authentication from configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConfig = builder.Configuration.GetSection("Redis");
    var redisHost = redisConfig.GetValue<string>("Host");
    var redisPort = redisConfig.GetValue<string>("Port");
    var redisPassword = redisConfig.GetValue<string>("Password");

    options.Configuration = $"{redisHost}:{redisPort},Password={redisPassword}";
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

app.UseAuthorization();

app.MapControllers();

app.Run();
