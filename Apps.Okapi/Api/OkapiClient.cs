using System.Xml.Serialization;
using Apps.Okapi.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Okapi.Api;

public class OkapiClient : RestClient
{
    public async Task<T> ExecuteWithXml<T>(string endpoint, Method method, object? bodyObj, AuthenticationCredentialsProvider[] creds)
    {
        var response = await Execute(endpoint, method, bodyObj, creds);

        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(response.Content);
        var result = (T)serializer.Deserialize(reader)!;
        
        return result;
    }
    
    public async Task<RestResponse> Execute(string endpoint, Method method, object? bodyObj,
        AuthenticationCredentialsProvider[] creds)
    {
        var baseUrl = creds.Get(CredsNames.Url).Value
            .TrimEnd('/');
        
        var request = new OkapiRequest(new()
        {
            Url = baseUrl + endpoint,
            Method = method
        });

        return await ExecuteRequest(request);
    }
    
    public async Task<RestResponse> ExecuteRequest(OkapiRequest request)
    {
        var response = await ExecuteAsync(request);

        if (!response.IsSuccessStatusCode)
            throw GetError(response);

        return response;
    }
    
    private Exception GetError(RestResponse response)
    {
        try
        {
            return new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
        catch (Exception e)
        {
            return new Exception($"Status code: {response.StatusCode}, Message: {response.Content}");
        }
    }
}