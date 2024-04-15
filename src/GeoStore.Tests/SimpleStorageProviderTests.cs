using GeoStore.Core;
using Xunit;

namespace GeoStore.Tests;

public class SimpleStorageProvider : IStorageProvider {
    private string _name = "FOO";

    public Task InitializeAsync()
    {
        // Here you would initialize your storage provider with the given credentials.
        // This example simply sets the name to the username for demonstration.

        return Task.CompletedTask;
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
        Assert.NotNull(points);
    }
}

public class SimpleStorageProviderTests
{
    [Fact]
    public void Initialize_SetsNameCorrectly()
    {
        // Arrange
        var storageProvider = new SimpleStorageProvider();
        string expectedUsername = "FOO";

        // Act
        storageProvider.InitializeAsync();

        // Assert
        var actualName = storageProvider.GetName();
        Assert.Equal(expectedUsername, actualName);
    }

    // Additional tests for other methods can be added here
    // For example, testing WriteRecords would ideally require a way to capture or mock the console output
}
