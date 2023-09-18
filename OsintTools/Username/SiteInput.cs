using SystemJson = System.Text.Json;

using OsintTools.Username.Interfaces;

using Newton = Newtonsoft.Json;

namespace OsintTools.Username;


/// <summary>
/// Reads sites' urls from json file
/// </summary>
public class LocalSiteInput : ISiteReader
{
    private readonly string _sitesJsonPath; 

    public LocalSiteInput(string pathToWebsiteData)
    {
        _sitesJsonPath = pathToWebsiteData;
    }
    
    /// <summary>
    /// Reads all sites from file at once
    /// </summary>
    /// <returns>List of SiteInfo's</returns>
    public List<SiteInfo> ReadAllSites()
    {
        string sitesJson = File.ReadAllText(_sitesJsonPath);
        List<SiteInfo> sites = SystemJson.JsonSerializer.Deserialize<List<SiteInfo>>(sitesJson);
        return sites;
    }

    /// <summary>
    /// Reads sites on demand without loading them all into memory
    /// </summary>
    /// <returns>List of SiteInfo's</returns>
    public IEnumerable<SiteInfo> YieldSites()
    {
        SiteInfo site;
        Newton.JsonSerializer serializer = new Newton.JsonSerializer();
        
        // read json items one by one for the sake of saving memory space
        using(FileStream fStream = File.Open(_sitesJsonPath, FileMode.Open))
            using(StreamReader readStream = new StreamReader(fStream))
            using (Newton.JsonReader jsonReader = new Newton.JsonTextReader(readStream))
            {
                // skip first json token: "{" or "["
                jsonReader.Read();
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == Newton.JsonToken.StartObject)
                    {
                        site = serializer.Deserialize<SiteInfo>(jsonReader);
                        yield return site;
                    }
                }
            }
    }
    
}
