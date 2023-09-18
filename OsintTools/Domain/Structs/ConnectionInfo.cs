namespace OsintTools.Domain.Structs;

public struct ConnectionInfo
{
    public int Asn { get; set; }
    public string Isp { get; set; }

    public override string ToString()
    {
        string templateString = ($"\nasn: {Asn}\n" +
                                 $"isp: {Isp}\n" +
                                 $"\n");

        return templateString;
    }
}