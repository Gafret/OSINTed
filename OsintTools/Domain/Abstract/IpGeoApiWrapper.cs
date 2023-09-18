using System.Net;
using OsintTools.Domain.Structs;

namespace OsintTools.Domain.Abstract;

public abstract class IpGeoApiWrapper
{
    protected string _apiKey;

    protected IpGeoApiWrapper(string apiKey)
    {
        _apiKey = apiKey;
    }

    public abstract IpGeoInfo GetGeoInfoFromIp(IPAddress ip);
}