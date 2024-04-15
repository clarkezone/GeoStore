
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
}