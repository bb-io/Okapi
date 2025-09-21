using System.Xml.Serialization;
using Apps.Okapi.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Okapi.Api;

public class OkapiClient : RestClient
{
    public async Task<T> ExecuteWithXml<T>(string endpoint, Method method, object? bodyObj, AuthenticationCredentialsProvider[] creds)
    {
        var response = await Execute(endpoint, method, bodyObj, creds);

        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(response.Content ?? string.Empty);
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

    public async Task<RestResponse> UploadFile(string endpoint, Method method, FileParameter fileParam,
        AuthenticationCredentialsProvider[] creds, List<Parameter>? formParameters = null)
    {
        var baseUrl = creds.Get(CredsNames.Url).Value
            .TrimEnd('/');

        var request = new OkapiRequest(new()
        {
            Url = baseUrl + endpoint,
            Method = method
        });

        request.AddFile(fileParam.Name, () => fileParam.GetFile(), fileParam.FileName, fileParam.ContentType);

        formParameters?.ForEach(parameter => request.AddParameter(parameter));

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
          throw new PluginApplicationException($"Status code: {response.StatusCode}, Content: {response.Content}");
    }
}