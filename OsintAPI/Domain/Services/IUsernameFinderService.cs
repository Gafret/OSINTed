using OsintTools.Username;

namespace OsintAPI.Domain.Services;

public interface IUsernameFinderService
{ 
    Task<IEnumerable<(string, UserRequestResult)>> SearchUsernameAsync(string userName);
}