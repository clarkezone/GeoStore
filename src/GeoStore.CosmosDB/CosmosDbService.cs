using GeoStore.Core;
using Microsoft.Azure.Cosmos;
using System.Text.Json.Serialization;

namespace GeoStore.CosmosDB;

public class DAOSample // current representation in COSMOSDB
{
    [JsonPropertyName("id")]
    public string id { get; set; }

    [JsonPropertyName("partitionid")]
    public string PartitionID { get; set; }

    // No JSON property attribute needed if the C# property name matches the JSON property name
    public double BatteryLevel { get; set; }
    public int Altitude { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }

    // Assuming you want these to match JSON properties with the same name as the C# property
    public string BatteryState { get; set; }
    public DateTime? Timestamp { get; set; }
    public string Wifi { get; set; }

    public static List<DAOSample> FromRootObject(RootObject rootObject)
    {
        List<DAOSample> dsl = new List<DAOSample>();
        foreach (var l in rootObject.Locations)
        {
            var ds = new DAOSample();
            ds.id = Guid.NewGuid().ToString();
            ds.PartitionID = "1";
            ds.BatteryLevel = Convert.ToDouble(l.Properties.BatteryLevel);
            ds.BatteryState = l.Properties.BatteryState;
            ds.Timestamp = l.Properties.Timestamp;
            ds.Lat = l.Geometry.Coordinates[0];
            ds.Lon = l.Geometry.Coordinates[1];
            ds.Altitude = Convert.ToInt32(l.Properties.Altitude);
            ds.Wifi = l.Properties.Wifi;
            dsl.Add(ds);
        }
        return dsl;
    }
}


public class CosmosDbService
{
    private CosmosClient _cosmosClient;
    private Container? _container;
    private string _databaseName;
    private string _containerName;

    public Container GetCurrentContainer()
    {
        return _container;
    }

    public CosmosDbService(CosmosClient cosmosClient, string containerName, string databaseName)
    {
        _containerName = containerName;
        _databaseName = databaseName;
        _cosmosClient = cosmosClient;
    }

    public CosmosDbService(string accountEndpoint, string authKey, string containerName, string databaseName)
    {
        //TODO: only for testing
        CosmosClientOptions options = new()
        {
            HttpClientFactory = () => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }),
            ConnectionMode = ConnectionMode.Gateway,
        };
        _containerName = containerName;
        _databaseName = databaseName;
        _cosmosClient = new CosmosClient(accountEndpoint: accountEndpoint, authKeyOrResourceToken: authKey, clientOptions: options);
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
        //This should return a collection of DAOSample objects
        var sampleList = DAOSample.FromRootObject(rootObject);

        //This should be a loop that adds each DAOSample object to the CosmosDB
        foreach (var item in sampleList)
        {
            await _container.CreateItemAsync(item);
        }
    }

    public async Task ResetContainer()
    {
        if (_container != null) {}
            var resp = await _container.DeleteContainerAsync();
            _container = null;
            await CreateContainerIfNotExistsAsync();
    }
}
