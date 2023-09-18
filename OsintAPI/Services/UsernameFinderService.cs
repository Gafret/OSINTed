using OsintAPI.Domain.Services;
using OsintTools;
using OsintTools.Username;

namespace OsintAPI.Services;

public class UsernameFinderService : IUsernameFinderService
{
    public async Task<IEnumerable<(string, UserRequestResult)>> SearchUsernameAsync(string userName)
    {
        // implement reading path from config file
        LocalSiteInput siteReader = new LocalSiteInput("D:\\Coding\\C#\\OsintAPI\\OsintTools" +
                                                       "\\bin\\Debug\\net7.0\\Username\\data\\data.json");
        UsernameFinder finder = new UsernameFinder(userName, siteReader);
        IEnumerable<(string, UserRequestResult)> userRequests = await finder.CheckAllSitesParallel();
        return userRequests;
    }
}