using Microsoft.AspNetCore.Mvc;
using OsintAPI.Domain.Services;
using OsintAPI.Resources;
using OsintAPI.Resources.Mappers;
using OsintTools.Username;

namespace OsintAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsernameFinderController : ControllerBase
{
    private readonly IUsernameFinderService _usernameFinderService;

    public UsernameFinderController(IUsernameFinderService finderService)
    {
        _usernameFinderService = finderService;
    }

    [HttpPost]
    public async Task<UserRequestResource[]> FindUser([FromBody] string userName)
    {
        IEnumerable<(string, UserRequestResult)> results = await _usernameFinderService.SearchUsernameAsync(userName);
        UserRequestResource[] resources = new UserRequestResource[results.Count()];
        for (int i = 0; i < results.Count(); i++)
        {
            (string, UserRequestResult) request = results.ElementAt(i);
            resources[i] = UserRequestMapper.MapToUserResource(request);
        }
        return resources;
    }
}