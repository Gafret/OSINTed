using System.Text.Json.Serialization;

namespace OsintTools.Domain.Structs;

public struct TimeZoneInfo
{
    public string Id { get; set; }
    
    [JsonPropertyName("current_time")]
    public DateTime CurrentTime { get; set; }
    
    [JsonPropertyName("gmt_offset")]
    public int GmtOffset { get; set; }

    public override string ToString()
    {
        string templateString = ($"\nid: {Id}\n" +
                                 $"time: {CurrentTime}\n" +
                                 $"gmt: {GmtOffset}\n" +
                                 $"\n");
        return templateString;
    }
}