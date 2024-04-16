namespace GeoStore.Core;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GeoStoreCore
{

    // [JsonSerializable(typeof(LocationFeature))]
    // [JsonSerializable(typeof(Geometry))]
    // [JsonSerializable(typeof(RootObject))]
    // [JsonSerializable(typeof(Properties))]
    // [JsonSerializable(typeof(Trip))]
    // [JsonSourceGenerationOptions(
    //     PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    //     GenerationMode = JsonSourceGenerationMode.Metadata)]
    // public partial class AppJsonContext : JsonSerializerContext
    // {
    // }

    private List<IStorageProvider> StorageProviders = new List<IStorageProvider>();

    public void AddProvider(IStorageProvider provider)
    {
        StorageProviders.Add(provider);
    }

    public RootObject HandlePost(string jsonPayload)
    {
        var rootObject = GetRootObject(jsonPayload);
        //TODO: null checking of providers and rootobject
        foreach (var provider in StorageProviders)
        {
            provider.WriteRecords(rootObject);
        }
        return rootObject;
    }

    public async Task InitializeAsync()
    {
        foreach (var provider in StorageProviders)
        {
            await provider.InitializeAsync();
            Console.WriteLine("Initializing provider: " + provider.GetName());
        }
    }

    public RootObject GetRootObject(string jsonPayload)
    {
        // var rootObject = JsonSerializer.Deserialize(
        //         jsonString, typeof(RootObject), SourceGenerationContext.Default.RootObject)
        //         as RootObject;

        var rootObject = JsonSerializer.Deserialize<RootObject>(jsonPayload, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (rootObject == null)
        {
            throw new ArgumentException("Invalid JSON payload", nameof(jsonPayload));
        }
        return rootObject;
    }
}