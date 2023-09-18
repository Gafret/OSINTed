using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Xml.Schema;
using OsintTools.Username;

namespace OsintTools.Domain;

public class WebsiteInvestigator
{
    private readonly string _host;

    public WebsiteInvestigator(SiteInfo website)
    {
        Uri siteUri = new Uri(website.MainUrl);
        _host = siteUri.Host;
    }

    public WebsiteInvestigator(string hostName)
    {
        _host = hostName;
    }

    // TODO: add support for multilayered tlds
    private string GetTld()
    {
        string tld;
        if (!string.IsNullOrEmpty(_host))
        {
            string[] domainParts = _host.Split(".");
            tld = domainParts.Last();
        }
        else
        {
            throw new NullReferenceException(
                "Host string is empty, check how you instantiated WebsiteInvestigator"
                );
        }

        return tld;
    }

    private static string? ParseWhois(string response)
    {
        string[] lines = response.Split(
            "\n",
            StringSplitOptions.RemoveEmptyEntries
        );
        foreach (var line in lines)
        {
            if (line.StartsWith("whois:"))
            {
                string[] entries = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                string whoisServer = entries.Last();
                return whoisServer;
            }
        }
        
        return null;
    }
    private string? GetWhoisServer(string rootWhoisAggregator = "whois.iana.org", int whoisPort = 43)
    {
        string tld = GetTld();

        using TcpClient tcpClient = new TcpClient(rootWhoisAggregator, whoisPort);
        NetworkStream stream = tcpClient.GetStream();
        
        byte[] messageInBytes = Encoding.ASCII.GetBytes($"{tld}\r\n");
        stream.Write(messageInBytes, 0, messageInBytes.Length);
        
        Byte[] buffer = new Byte[1024];

        string response = "";
        
        Int32 bytes = stream.Read(buffer, 0, buffer.Length);
        while (bytes > 0)
        {
            response = Encoding.ASCII.GetString(buffer, 0, bytes);
            string? whoisServer = ParseWhois(response);
            if (whoisServer is not null)
                return whoisServer;
            
            bytes = stream.Read(buffer, 0, buffer.Length);
        }

        return null;
    }

    // TODO: refactoring
    private string GetWhois()
    {
        string? whoisServer = GetWhoisServer();
        if (whoisServer is null)
            throw new NullReferenceException("Whois server is not found");
        
        int portNumber = 43;
        using TcpClient client =  new TcpClient(whoisServer, portNumber);
        NetworkStream stream = client.GetStream();
        
        byte[] messageInBytes = Encoding.ASCII.GetBytes($"{_host}\r\n");
        stream.Write(messageInBytes, 0, messageInBytes.Length);
        
        Byte[] buffer = new Byte[1024];

        string response = "";
        
        Int32 bytes = stream.Read(buffer, 0, buffer.Length);
        while (bytes > 0)
        {
            response += Encoding.ASCII.GetString(buffer, 0, bytes);
            bytes = stream.Read(buffer, 0, buffer.Length);
        }

        return response;
    }

    public string GetIpHistory()
    {
        return "";
    }

    public IEnumerable<PingReply> TraceRoute(int timeOut, int maxTtl)
    {
        const int bufferSize = 32;

        byte[] buffer = new byte[bufferSize];

        using Ping pinger = new Ping();
        for (int ttl = 1; ttl <= maxTtl; ttl++)
        {
            PingOptions options = new PingOptions(ttl, true);
            PingReply reply = pinger.Send(_host, timeOut, buffer, options);

            if (reply.Status == IPStatus.TtlExpired)
            {
                yield return reply;
            } else if (reply.Status == IPStatus.TimedOut)
            {
                yield return reply;
            } else if (reply.Status == IPStatus.Success)
            {
                yield return reply;
                yield break;
            }
        }
    }

    private PingReply Ping()
    {
        using Ping pinger = new Ping();
        var pingResponse = pinger.Send(_host);
        
        return pingResponse;
    }

    public string GetAbuseEmail()
    {
        return "";
    }

    public async Task<IEnumerable<Tuple<int, bool>>> CheckPortsAsync(IEnumerable<int> ports)
    {
        int portCount = ports.Count();
        Task<bool>[] pendingConnectionRequests = new Task<bool>[portCount];

        for (int i = 0; i < portCount; i++)
        {
            Task<bool> pendingConnection = TcpConnectToPortAsync(ports.ElementAt(i));
            pendingConnectionRequests[i] = pendingConnection;
        }

        IEnumerable<bool> responses = await Task.WhenAll(pendingConnectionRequests);
        IEnumerable<Tuple<int, bool>> portConnections = ports.Zip<int, bool, Tuple<int, bool>>(responses, 
            (port, response) => new Tuple<int, bool>(port, response));
        
        return portConnections;
    }

    private async Task<bool> TcpConnectToPortAsync(int port)
    {
        using TcpClient tcpClient = new TcpClient();
        try
        {
            await tcpClient.ConnectAsync(_host, port);
        }
        catch (SocketException socketException)
        {
            return false;
        }
        return true;
    }

    public bool CheckIsActive()
    {
        bool isActive = Ping().Status == IPStatus.Success;
        return isActive;
    }
    
    public string GetSubDomains()
    {
        return "";
    }
    
    
}