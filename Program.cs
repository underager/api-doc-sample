using APIDocumentationSample.BusinessLogic;
using APIDocumentationSample.DataAccess;
using APIDocumentationSample.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Add Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
var multiplexer = ConnectionMultiplexer.Connect(redisConnection);
builder.Services.AddSingleton(multiplexer);

// Add Data Access and Business Logic layers
var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
builder.Services.AddScoped<ICarDataAccess>(sp => 
    new CarDataAccess(sqlConnectionString, sp.GetRequiredService<ILogger<CarDataAccess>>()));
builder.Services.AddScoped<ICarBusinessLogic, CarBusinessLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
