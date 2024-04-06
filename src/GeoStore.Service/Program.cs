using GeoStore.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable JSON body binding for minimal APIs
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/payload", async (HttpRequest request) =>
{
    Console.WriteLine("Request received\n");
    using var reader = new StreamReader(request.Body);
    var jsonPayload = await reader.ReadToEndAsync();

    //TODO: configure with providers
    var core = new GeoStoreCore();

    // Ensure the directory exists
    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "payloads.txt");
    //await File.AppendText(filePath, "***);
    //await File.AppendAllTextAsync(filePath, jsonPayload + Environment.NewLine);
    var rootObject = core.HandlePost(jsonPayload);

    return Results.Ok(new { result = "ok" });
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
