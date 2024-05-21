namespace GeoStore.CosmosDB;

using GeoStore.Core;
using System.Text.Json.Serialization;


public class CosmosSimpleStorageProvider : IStorageProvider
{
    CosmosDbService _cosmosDbService;

    public CosmosSimpleStorageProvider(string accountEndpoint, string authKey, string databaseName, string containerName)
    {
        //TODO read these from the environment
       _cosmosDbService = new CosmosDbService(accountEndpoint, authKey, databaseName, containerName); 
    }

    public async Task InitializeAsync()
    {
        await _cosmosDbService.InitAsync();
    }

    public string GetName()
    {
        // Returns the name of the storage provider.
        return "CosmosSimple";
    }

    public async Task WriteRecords(RootObject rootObject)
    {
        if (rootObject == null)
        {
            Console.WriteLine("Can't write records Null");
        }
        else
        {
            Console.WriteLine($"CosmosSimpleProvider: /{rootObject}");
            var ds = DAOSample.FromRootObject(rootObject);
            await _cosmosDbService.AddRootObjectAsync(rootObject);
        }
    }
}