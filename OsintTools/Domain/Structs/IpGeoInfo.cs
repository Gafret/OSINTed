using System.Text.Json.Serialization;

namespace OsintTools.Domain.Structs;

public struct IpGeoInfo
{
    public string City { get; set; }
    
    [JsonPropertyName("country_name")]
    public string Country { get; set; }
    
    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }
    
    public ConnectionInfo ConnectionInfo { get; set; }
    
    [JsonPropertyName("latitude")]
    public double Lat { get; set; }
    
    [JsonPropertyName("longitude")]
    public double Lon { get; set; }
    
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; }
    
    [JsonPropertyName("region_name")]
    public string RegionName { get; set; }
    
    [JsonPropertyName("time_zone")]
    public TimeZoneInfo Timezone { get; set; }
    
    [JsonPropertyName("zip")]
    public string Zip { get; set; }

    public override string ToString()
    {
        string templateString = ($"city: {City}\n" +
                                 $"country: {Country}\n" +
                                 $"connection: '{ConnectionInfo}'\n" +
                                 $"latitude: {Lat}\n" +
                                 $"longitude: {Lon}\n" +
                                 $"region: {RegionName}\n" +
                                 $"timezone: '{Timezone}'\n" +
                                 $"zip: {Zip}\n" +
                                 $"\n");
        return templateString;
    }
}