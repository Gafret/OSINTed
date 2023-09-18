using System.Net;
using System.Net.Sockets;

using OsintTools.Username;
using OsintTools.Username.Interfaces;

namespace OsintTools;

/// <summary>
/// Contains all methods for requesting user existence
/// on websites
/// </summary>
public class UsernameFinder
{
    private static readonly HttpClient _sharedClient;

    private string _userName;
    private ISiteReader _siteReader;

    public UsernameFinder(string userName, ISiteReader siteReader)
    {
        _userName = userName;
        _siteReader = siteReader;
    }

    static UsernameFinder()
    {
        _sharedClient = new HttpClient(
            new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5), 
                ConnectTimeout = TimeSpan.FromMinutes(5) 
            }
        );
    }
    
    
    /// <summary>
    /// Checks if there is 'userName' registered on website
    /// </summary>
    /// <param name="userName">Username to be checked for</param>
    /// <param name="website">SiteInfo with UserUrl</param>
    /// <param name="requestHeaders">Custom headers for requests</param>
    /// <returns>Tuple with website url and request result as enum</returns>
    public async Task<(string, UserRequestResult)> SearchUsernameOnSite(
        SiteInfo website, 
        Dictionary<string, string>? requestHeaders = null
        ) 
    {
        if (requestHeaders is not null)
        {
            foreach (var (key, value) in requestHeaders)
            {
                _sharedClient.DefaultRequestHeaders.Add(key, value); // unsafe static changes gloabal variable 'client' - not good
            }
        }

        string websiteUserUrl = website.UserUrl;
        websiteUserUrl = websiteUserUrl.Replace("{}", _userName);
        HttpResponseMessage response = await _sharedClient.GetAsync(websiteUserUrl);

        _sharedClient.DefaultRequestHeaders.Clear();
        
        UserRequestResult userRequestResult = UserRequestResult.RequestError;
        
        if (response.StatusCode == HttpStatusCode.OK) userRequestResult = UserRequestResult.HasUser;
        else if (response.StatusCode == HttpStatusCode.NotFound) userRequestResult = UserRequestResult.HasNotUser;

        return (website.MainUrl, userRequestResult);
    }
    
    /// <summary>
    /// Makes requests to websites in parallel to check if user exists
    /// </summary>
    /// <param name="userName">Username to be checked for</param>
    /// <param name="siteReader">Class that implements any access to website lists</param>
    /// <returns>Enumerable with tuples consisting of website urls and request results</returns>
    public async Task<IEnumerable<(string, UserRequestResult)>> CheckAllSitesParallel()
    {
        List<Task<(string, UserRequestResult)>> requestsToSites = new List<Task<(string, UserRequestResult)>>();
        Task aggregateTask = null;

        foreach (var site in _siteReader.YieldSites())
        {
            Task<(string, UserRequestResult)> requestToSite = SearchUsernameOnSite(site);
            requestsToSites.Add(requestToSite);
        }

        try
        {
            aggregateTask = Task.WhenAll(requestsToSites);
            await aggregateTask;
        }
        catch (Exception ex) when (ex is HttpRequestException ||
                                   ex is SocketException ||
                                   ex is AggregateException)
        {
            if (aggregateTask?.Exception?.InnerExceptions != null &&
                aggregateTask.Exception.InnerExceptions.Any())
            {
                foreach (var innerExc in aggregateTask.Exception.InnerExceptions)
                {
                    // handle somehow
                    Console.WriteLine(innerExc.Message);
                }
            }
        }

        IEnumerable<(string, UserRequestResult)> requestResults =
            from request in requestsToSites
            where request.IsCompletedSuccessfully
            select request.Result;

        return requestResults;
    }
    
    /// <summary>
    /// Asynchronously requests for user existence on websites
    /// </summary>
    /// <param name="userName">Username to be checked for</param>
    /// <param name="siteReader">Class that implements any access to website lists</param>
    /// <returns>Async Enumerable with tuples consisting of website urls and request results</returns>
    public async IAsyncEnumerable<(string, UserRequestResult)> CheckAllSitesAsync()
    {
        IEnumerable<SiteInfo> sitesSequence = _siteReader.YieldSites();
        UserRequestResult isUsernameOnSite = UserRequestResult.HasNotUser;
        string site = "None";
        foreach (SiteInfo siteData in sitesSequence)
        {
            try
            {
                (site, isUsernameOnSite) = await SearchUsernameOnSite(siteData);
            }
            catch (Exception ex) when (ex is HttpRequestException ||
                                       ex is SocketException)
            {
                (site, isUsernameOnSite) =
                    (siteData.MainUrl, UserRequestResult.RequestError); // inconsistent with parallel request
                Console.WriteLine(
                    $"Exception '{ex.Message}' on site '{siteData.MainUrl}' for '{_userName}'");
            }
            yield return (site, isUsernameOnSite);
        }
    }

} 