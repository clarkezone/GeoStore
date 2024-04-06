namespace GeoStore.Core;
using System.Text.Json;

public class GeoStoreCore
{
    public RootObject HandlePost(string jsonPayload)
    {
        var rootObject = JsonSerializer.Deserialize<RootObject>(jsonPayload);
        return rootObject;
    }
}