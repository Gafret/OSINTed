using OsintTools.Username.Interfaces;

namespace OsintTools.Username;

/// <summary>
/// Class used for deserializing json website data
/// </summary>
public class SiteInfo : ISiteInfo
{
    [Newtonsoft.Json.JsonProperty("url")]
    public string UserUrl { get; set; }
    [Newtonsoft.Json.JsonProperty("urlMain")]
    public string MainUrl { get; set; }
    [Newtonsoft.Json.JsonProperty("isNSFW")]
    public bool IsNSFW { get; set; }

    public override string ToString()
    {
        string repr = $"UserURL: {UserUrl}, MainURL: {MainUrl}";
        return repr;
    }
}