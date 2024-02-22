using RestSharp;

namespace Apps.Okapi.Api;

public class OkapiRequest : RestRequest
{
    public OkapiRequest(OkapiRequestParameters requestParameters) : base(requestParameters.Url, requestParameters.Method)
    {
    }
}