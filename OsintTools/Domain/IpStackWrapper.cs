using System.Net;
using System.Net.Http.Json;

using OsintTools.Domain.Abstract;
using OsintTools.Domain.Exceptions;
using OsintTools.Domain.Structs;

namespace OsintTools.Domain;

public class IpStackWrapper : IpGeoApiWrapper
{
    private static readonly HttpClient _httpClient;
        
    public IpStackWrapper(string apiKey) : base(apiKey){}

    static IpStackWrapper()
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://api.ipstack.com/"),
        };
    }
    
    public override IpGeoInfo GetGeoInfoFromIp(IPAddress ip)
    {
        string requestTemplate = $"{ip}?access_key={_apiKey}";
        IpGeoInfo ipGeoInfo = _httpClient.GetFromJsonAsync<IpGeoInfo>(requestTemplate).Result;
        return ipGeoInfo;
    }

    public IpGeoInfo[] GetGeoInfoFromIp(string hostName)
    {
        IPAddress[] hostIpAddresses = ConvertHostToIpAddress(hostName);
        IpGeoInfo[] geoInfoForAllIps = new IpGeoInfo[hostIpAddresses.Length];
        for(int i=0; i<hostIpAddresses.Length; i++)
        {
            IpGeoInfo ipGeoInfo = GetGeoInfoFromIp(hostIpAddresses[i]);
            geoInfoForAllIps[i] = ipGeoInfo;
        }
        return geoInfoForAllIps;
    }

    private static IPAddress[] ConvertHostToIpAddress(string hostName)
    {
        IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
        if (hostEntry.AddressList.Length > 0)
        {
            return hostEntry.AddressList;
        }

        throw new HostIpNotFoundException();
    }
}