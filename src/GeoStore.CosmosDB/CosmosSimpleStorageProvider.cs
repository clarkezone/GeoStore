namespace GeoStore.CosmosDB;

using GeoStore.Core;

public class Class1
{

}

public class CosmosSimpleStorageProvider : IStorageProvider
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