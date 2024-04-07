namespace GeoStore.Core;

using System;
using System.Text.Json;

public class GeoStoreCore
{
    private List<IStorageProvider> StorageProviders = new List<IStorageProvider>();

    public void AddProvider(IStorageProvider provider)
    {
        StorageProviders.Add(provider);
    }

    public RootObject HandlePost(string jsonPayload)
    {
        var rootObject = JsonSerializer.Deserialize<RootObject>(jsonPayload);
        //TODO: null checking of providers and rootobject
        foreach (var provider in StorageProviders) {
            provider.WriteRecords(rootObject);
        }
        return rootObject;
    }

    public void Initialize(string v1, string v2)
    {
        foreach (var provider in StorageProviders) {
            provider.Initialize(v1, v2);
            Console.WriteLine("Initializing provider: " + provider.GetName());
        }
    }
}