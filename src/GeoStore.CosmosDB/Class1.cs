namespace GeoStore.CosmosDB;

using GeoStore.Core;

public class Class1
{

}

public class SimpleStorageProvider : IStorageProvider
{
    private string _name;

    public void Initialize(string username, string password)
    {
        // Here you would initialize your storage provider with the given credentials.
        // This example simply sets the name to the username for demonstration.
        _name = username;
    }

    public string GetName()
    {
        // Returns the name of the storage provider.
        return _name;
    }

    public void WriteRecords(RootObject points)
    {
        // Here you would implement the logic to write the points to the storage.
        // This example simply prints the points to the console for demonstration.
        foreach (var point in points)
        {
            Console.WriteLine($"Writing point: ({point.X}, {point.Y})");
        }
    }
}