namespace GeoStore.CosmosDB;

using GeoStore.Core;

public class CosmosSimpleStorageProvider : IStorageProvider
{
    CosmosDbService _cosmosDbService;

    public CosmosSimpleStorageProvider(string accountEndpoint, string authKey)
    {
       _cosmosDbService = new CosmosDbService(accountEndpoint, authKey, "CONTAINER", "DATABASE"); 
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

    public void WriteRecords(RootObject rootObject)
    {
        if (rootObject == null)
        {
            Console.WriteLine("Can't write records Null");
        }
        else
        {
            Console.WriteLine($"CosmosSimpleProvider: /{rootObject}");
        }
    }
}