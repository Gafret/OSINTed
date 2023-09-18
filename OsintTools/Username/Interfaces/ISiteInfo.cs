namespace OsintTools.Username.Interfaces;

public interface ISiteInfo
{
    // main url and user url addresses are separated in case
    // they lay in different domains or something similar
    public string UserUrl { get; set; }
    public string MainUrl { get; set; }
}