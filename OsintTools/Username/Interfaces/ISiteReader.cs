namespace OsintTools.Username.Interfaces;

public interface ISiteReader
{
    IEnumerable<SiteInfo> YieldSites();
    List<SiteInfo> ReadAllSites();
}