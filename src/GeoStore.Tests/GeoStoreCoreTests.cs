using System.Text.Json;
using GeoStore.Core;

namespace GeoStore.Tests;

public class SerializationTests
{
    [Fact]
    public void TestDeserializationOfJsonFragments()
    {
        var fileName = "TestData//payloads.json"; // The name of your file
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

        // Now you can use filePath as the path to your file
        Assert.True(File.Exists(filePath), "Expected data file not found.");

        var jsonContent = File.ReadAllText(filePath);
        var jsonFragments = jsonContent.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
        var gs = new GeoStoreCore();

        // Act
        var rootObjects = jsonFragments.Select(fragment => gs.GetRootObject(fragment)).ToList();

        // Assert
        Assert.NotNull(rootObjects);
        Assert.All(rootObjects, item => Assert.NotNull(item));

        Assert.All(rootObjects, item => Assert.NotNull(item.Locations));

        Assert.NotNull(rootObjects[0].Locations[0].Properties.BatteryLevel);
        // Assert.All(rootObjects, item => Assert.NotNull(item.Locations[0].Properties.BatteryLevel));
        //TODO: fix this make these test better
        //Assert.True(rootObjects.All(ro => ro.Locations != null)); // Example assertion, adjust as needed

        // Additional assertions as per your requirements
    }
}