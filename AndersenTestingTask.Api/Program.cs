using System.Threading.RateLimiting;
using AndersenTestingTask.Domain.Repositories;
using AndersenTestingTask.Domain.Repositories.Interfaces;
using AndersenTestingTask.Domain.Services;
using AndersenTestingTask.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixedPolicy", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));
builder.Services.AddCors(options =>
    options.AddPolicy("AllowClientApp", p => p
        .WithOrigins("https://localhost:7132")
        .AllowCredentials()
        .AllowAnyMethod()
        .AllowAnyHeader()));
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IDataProvider, MockyDataProvider>();
builder.Services.AddTransient<IProductService, ProductsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors();


app.UseRateLimiter();

static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

app.MapGet("/", () => Results.Ok($"Fixed Window Limiter {GetTicks()}"))
    .RequireRateLimiting("fixed");

app.Run();