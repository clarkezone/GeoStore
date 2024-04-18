namespace GeoStore.Core;
public interface IStorageProvider
{
    // Initializes the storage provider with credentials.
    Task InitializeAsync();

    // Returns the name of the storage provider.
    string GetName();

    // Writes a collection of points to the storage.
    Task WriteRecords(RootObject points);
}
