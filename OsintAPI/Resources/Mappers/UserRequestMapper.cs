using OsintTools.Username;

namespace OsintAPI.Resources.Mappers;

public class UserRequestMapper
{
    public static UserRequestResource MapToUserResource((string, UserRequestResult) requestResult)
    {
        string hasUser = "error occured";
        switch (requestResult.Item2)
        {
            case UserRequestResult.HasUser:
                hasUser = "found";
                break;
            case UserRequestResult.HasNotUser:
                hasUser = "not found";
                break;
        }

        UserRequestResource resource = new UserRequestResource()
        {
            WebsiteUrl = requestResult.Item1,
            HasUser = hasUser,
        };

        return resource;
    }
}