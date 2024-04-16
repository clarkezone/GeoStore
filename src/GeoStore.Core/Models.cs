namespace GeoStore.Core;
using System.Text.Json.Serialization;

public class RootObject
{
    public List<LocationFeature>? Locations { get; set; }
    public Trip? Trip { get; set; }
}

public class LocationFeature
{
    public string? Type { get; set; }
    public Geometry? Geometry { get; set; }
    public Properties? Properties { get; set; }
}

public class Geometry
{
    public string? Type { get; set; }
    public List<double>? Coordinates { get; set; }
}

public class Properties
{
    // Common properties
    public List<string>? Motion { get; set; }
    public double? Speed { get; set; }
    [JsonPropertyName("battery_level")]
    public double? BatteryLevel { get; set; }
    public double Altitude { get; set; }

    [JsonPropertyName("battery_state")]
    public string? BatteryState { get; set; }
    [JsonPropertyName("horizontal_accuracy")]
    public double HorizontalAccuracy { get; set; }
    [JsonPropertyName("vertical_accuracy")]
    public double VerticalAccuracy { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? Wifi { get; set; }

    // Properties specific to trips or more detailed data
    public string? Start { get; set; }
    public string? End { get; set; }
    [JsonPropertyName("end_location")]
    public LocationFeature? EndLocation { get; set; }
    public string? Type { get; set; }
    public bool? StoppedAutomatically { get; set; }
    public string? Mode { get; set; }
    public LocationFeature? StartLocation { get; set; }
    public double? Duration { get; set; }
    public double? Distance { get; set; }
    public string? DeviceId { get; set; }
    public int? Steps { get; set; }

    // Additional properties for diverse payloads
    public bool? Pauses { get; set; }
    public int? Deferred { get; set; }
    public int? SignificantChange { get; set; }
    public int? LocationsInPayload { get; set; }
    public string? Activity { get; set; }
    public double? DesiredAccuracy { get; set; }
}

public class Trip
{
    public double Distance { get; set; }
    public LocationFeature? StartLocation { get; set; }
    public LocationFeature? CurrentLocation { get; set; }
    public string? Mode { get; set; }
    public string? Start { get; set; }
}

