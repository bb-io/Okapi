using RestSharp;

namespace Apps.Okapi.Api;

public class OkapiRequestParameters
{
    public string Url { get; set; } = string.Empty;

    public Method Method { get; init; }
}