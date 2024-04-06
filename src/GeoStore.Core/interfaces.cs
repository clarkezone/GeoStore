namespace GeoStore.Core;
public interface IStorageProvider
{
    // Initializes the storage provider with credentials.
    void Initialize(string username, string password);

    // Returns the name of the storage provider.
    string GetName();

    // Writes a collection of points to the storage.
    void WriteRecords(RootObject points);
}
