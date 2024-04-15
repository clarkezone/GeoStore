
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
        client = new(
                   accountEndpoint: "https://localhost:8081/",
                   authKeyOrResourceToken: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
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

    [Fact]
    public async Task TestEmulator()
    {
        Database database = await fixture.client.CreateDatabaseIfNotExistsAsync(
            id: "cosmicworks",
            throughput: 400
        );

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

    public async Task TestEnsureDBCollections() {
        // Arrange
        var cosmosDbService = new CosmosDbService(fixture.client, "pointstore", "geostore-testdb");

        // Act
        await cosmosDbService.InitAsync();

        // Assert
        Assert.NotNull(cosmosDbService);
    }
}