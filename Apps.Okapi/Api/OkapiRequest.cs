using RestSharp;

namespace Apps.Okapi.Api;

public class OkapiRequest(OkapiRequestParameters requestParameters)
    : RestRequest(requestParameters.Url, requestParameters.Method);