using GeoStore.Core;
using GeoStore.CosmosDB;

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

var core = new GeoStoreCore();

// Read environment variables
string? accountEndpoint = Environment.GetEnvironmentVariable("ACCOUNT_ENDPOINT");
string? authKey = Environment.GetEnvironmentVariable("AUTH_KEY");

// Check if environment variables are set
if (string.IsNullOrEmpty(accountEndpoint) || string.IsNullOrEmpty(authKey))
{
    throw new Exception("Missing necessary environment variables.");
}

core.AddProvider(new CosmosSimpleStorageProvider(accountEndpoint, authKey));

await core.InitializeAsync();

app.MapPost("/api/payload", async (HttpRequest request) =>
{
    Console.WriteLine("Request received\n");
    using var reader = new StreamReader(request.Body);
    var jsonPayload = await reader.ReadToEndAsync();

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
