namespace GeoStore.Core;

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
    public double? BatteryLevel { get; set; }
    public double Altitude { get; set; }
    public string? BatteryState { get; set; }
    public double HorizontalAccuracy { get; set; }
    public double VerticalAccuracy { get; set; }
    public string? Timestamp { get; set; }
    public string? Wifi { get; set; }

    // Properties specific to trips or more detailed data
    public string? Start { get; set; }
    public string? End { get; set; }
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

