using GeoStore.Core;
using Microsoft.Azure.Cosmos;
using System.Text.Json.Serialization;

namespace GeoStore.CosmosDB;

public class DAOSample // current representation in COSMOSDB
{
    [JsonPropertyName("id")]
    public string ID { get; set; }

    [JsonPropertyName("partitionid")]
    public string PartitionID { get; set; }

    // No JSON property attribute needed if the C# property name matches the JSON property name
    public double BatteryLevel { get; set; }
    public int Altitude { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }

    // Assuming you want these to match JSON properties with the same name as the C# property
    public string BatteryState { get; set; }
    public string Timestamp { get; set; }
}


public class CosmosDbService
{
    private CosmosClient _cosmosClient;
    private Container? _container;
    private string _databaseName;
    private string _containerName;


    public CosmosDbService(CosmosClient cosmosClient, string containerName, string databaseName)
    {
        _containerName = containerName;
        _databaseName = databaseName;
        _cosmosClient = cosmosClient;
    }

    public CosmosDbService(string accountEndpoint, string authKey, string containerName, string databaseName)
    {
        _containerName = containerName;
        _databaseName = databaseName;
        _cosmosClient = new CosmosClient(accountEndpoint: accountEndpoint, authKeyOrResourceToken: authKey);
    }

    public async Task InitAsync()
    {
        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await CreateDatabaseIfNotExistsAsync();
        await CreateContainerIfNotExistsAsync();
    }

    public async Task CreateDatabaseIfNotExistsAsync()
    {
        try
        {
            var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
            Console.WriteLine($"Created Database: {databaseResponse.Database.Id}");
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"CreateDatabaseIfNotExistsAsync failed with {ex.StatusCode}:\n{ex}");
            throw;
        }
    }

    public async Task CreateContainerIfNotExistsAsync()
    {
        try
        {
            // Adjust the "/id" partition key path if your model uses a different property for partitioning
            var containerResponse = await _cosmosClient.GetDatabase(_databaseName).CreateContainerIfNotExistsAsync(_containerName, "/id");
            _container = containerResponse.Container;
            Console.WriteLine($"Created Container: {containerResponse.Container.Id}");
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"CreateContainerIfNotExistsAsync failed with {ex.StatusCode}:\n{ex}");
            throw;
        }
    }

    public async Task AddRootObjectAsync(RootObject rootObject)
    {
        DAOSample ds = new DAOSample();
        foreach (var l in rootObject.Locations)
        {
            foreach (var c in l.Geometry.Coordinates)
            {

            }
        }
        //ds.BatteryState = rootObject.Locations
        //_container.CreateItemAsync
        //        try
        //        {
        //            await _container.CreateItemAsync(rootObject, new PartitionKey(rootObject.Id));
        //        }
        //        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        //        {
        //            Console.WriteLine($"An item with the same id already exists: {ex.Message}");
        //        }
    }
}
