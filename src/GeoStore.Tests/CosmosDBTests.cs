
using System.Text.Json;
using GeoStore.Core;
using Xunit;
using Microsoft.Azure.Cosmos;
using GeoStore.CosmosDB;
namespace GeoStore.Tests;


public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        //TODO: start docker container

        CosmosClientOptions options = new()
        {
            HttpClientFactory = () => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }),
            ConnectionMode = ConnectionMode.Gateway,
        };

        client = new CosmosClient(
                accountEndpoint: "https://localhost:8081/",
                authKeyOrResourceToken: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                clientOptions: options
               );
    }

    public void Dispose()
    {
        client.Dispose();
    }

    public CosmosClient client { get; private set; }
}


[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}

[Collection("Database collection")]
public class CosmosDBTests
{
    DatabaseFixture fixture;
    public CosmosDBTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact,]
    public async Task TestEmulator()
    {
        Database? database = null;
        try
        {
            database = await fixture.client.CreateDatabaseIfNotExistsAsync(
                id: "cosmicworks",
                throughput: 400
            );
        }
        catch (Exception e)
        {
            throw new Exception("Please make sure the CosmosDB emulator is running using /scripts/start-cosmosdb-emulator.sh");
        }

        if (database != null)
        {
            Container container = await database.CreateContainerIfNotExistsAsync(
                id: "products",
                partitionKeyPath: "/id"
            );

            var item = new
            {
                id = "68719518371",
                name = "Kiama classic surfboard"
            };

            await container.UpsertItemAsync(item);

            using FeedIterator<Object> feed = container.GetItemQueryIterator<Object>(
                queryText: "SELECT * FROM products"
            );

            while (feed.HasMoreResults)
            {
                FeedResponse<Object> response = await feed.ReadNextAsync();
                Assert.Equal(1, response.Count);
            }
        }
    }

    [Fact]
    public async Task TestEnsureDBCollections()
    {
        // Arrange
        CosmosDbService cosmosDbService = GetDbServiceCheckEmulator("pointstore", "geostore-testdb");

        // Act
        await cosmosDbService.InitAsync();

        // Assert
        Assert.NotNull(cosmosDbService);
    }

    private CosmosDbService GetDbServiceCheckEmulator(string containerName, string dbName)
    {
        try
        {
            return new CosmosDbService(fixture.client, containerName, dbName);
        }
        catch (Exception)
        {
            throw new Exception("Please make sure the CosmosDB emulator is running using /scripts/start-cosmosdb-emulator.sh");
        }
    }

    // [Fact(Skip = "disabling until json deserialization is fixed")]
    [Fact]
    public void TestGetDAOSample()
    {
        var fileName = "TestData//payloads.json"; // The name of your file
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

        // Now you can use filePath as the path to your file
        Assert.True(File.Exists(filePath), "Expected data file not found.");

        var jsonContent = File.ReadAllText(filePath);
        var jsonFragments = jsonContent.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
        var gs = new GeoStoreCore();

        // Act
        var rootObjects = jsonFragments.Select(fragment => gs.GetRootObject(fragment)).ToList();
        var daoSample = DAOSample.FromRootObject(rootObjects[1]);

        // Assert
        Assert.NotNull(daoSample);
        Assert.NotNull(daoSample.Id); //TODO verify all fields in daosample are correct
        //TODO verify that the datetime is correct and matches the golang implementation 
        // Verify the values
        Assert.Equal(-122.11782266321981, daoSample.Locations[0].Geometry.Coordinates[0]);
        Assert.Equal(47.650458045770122, daoSample.Locations[0].Geometry.Coordinates[1]);
        Assert.Equal(-122.11782266321981, daoSample.Locations[1].Geometry.Coordinates[0]);
        Assert.Equal(47.650458045770122, daoSample.Locations[1].Geometry.Coordinates[1]);
        Assert.Equal(0.5, daoSample.Locations[0].Properties.BatteryLevel);
        Assert.Equal(0.5, daoSample.Locations[1].Properties.BatteryLevel);
        Assert.Equal(41, daoSample.Locations[0].Properties.Altitude);
        Assert.Equal(40, daoSample.Locations[1].Properties.Altitude);
        Assert.Equal("unplugged", daoSample.Locations[0].Properties.BatteryState);
        Assert.Equal("unplugged", daoSample.Locations[1].Properties.BatteryState);
        Assert.Equal(6, daoSample.Locations[0].Properties.HorizontalAccuracy);
        Assert.Equal(6, daoSample.Locations[1].Properties.HorizontalAccuracy);
        Assert.Equal(14, daoSample.Locations[0].Properties.VerticalAccuracy);
        Assert.Equal(14, daoSample.Locations[1].Properties.VerticalAccuracy);
        Assert.Equal(DateTime.Parse("2024-04-04T20:50:00Z"), daoSample.Locations[0].Properties.Timestamp);
        Assert.Equal(DateTime.Parse("2024-04-04T20:52:13Z"), daoSample.Locations[1].Properties.Timestamp);
        Assert.Equal("Fordyce MKII", daoSample.Locations[0].Properties.Wifi);
        Assert.Equal("Fordyce MKII", daoSample.Locations[1].Properties.Wifi);
    }

    [Fact]
    public void TestAddRootObject()
    {
        // Arrange
        var fileName = "TestData//payloads.json"; // The name of your file
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

        // Now you can use filePath as the path to your file
        Assert.True(File.Exists(filePath), "Expected data file not found.");

        var jsonContent = File.ReadAllText(filePath);
        var jsonFragments = jsonContent.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
        var gs = new GeoStoreCore();
        var rootObjects = jsonFragments.Select(fragment => gs.GetRootObject(fragment)).ToList();
        var cosmosDbService = GetDbServiceCheckEmulator("pointstore", "geostore-testdb");

        // Act
        cosmosDbService.AddRootObject(rootObjects[0]);

        // Assert
        Assert.NotNull(cosmosDbService);
        //this should verify the count of items in the container having nuked all items in that container for determinism
    }
}
