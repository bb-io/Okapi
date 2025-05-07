using Apps.Okapi.Api;
using Apps.Okapi.Constants;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Okapi.Invocables;

public class AppInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected OkapiClient Client { get; }

    public AppInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new();
    }

    protected async Task<string?> CreateNewProject()
    {
        var response = await Client.Execute(ApiEndpoints.Projects + "/new", Method.Post, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new PluginApplicationException($"Status code: {response.StatusCode}, Content: {response.Content}");
        }

        var locationHeader = response.Headers.FirstOrDefault(h => h.Name.Equals("Location", StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
        if (locationHeader == null)
        {
            throw new PluginApplicationException("Location header is missing in the response.");
        }

        var uri = new Uri(locationHeader);
        return uri.Segments.Last();
    }

    protected async Task AddBatchConfig(string projectId, byte[] fileBytes, string name = "batch.bconf", string contentType = "application/octet-stream")
    {
        string endpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.BatchConfiguration;
        var fileParam = FileParameter.Create("batchConfiguration", fileBytes, name, contentType);

        try
        {
            var response = await Client.UploadFile(endpoint, Method.Post, fileParam, Creds);
        }
        catch (PluginApplicationException e)
        {
            throw new PluginApplicationException("Could not upload your batch configuration file; " + e.Message);
        }
    }

    protected async Task UploadFile(string projectId, byte[] fileBytes, string fileName, string contentType)
    {
        if (string.IsNullOrEmpty(fileName)) throw new("File name is required.");

        var fileParam = FileParameter.Create("inputFile", fileBytes, fileName, contentType);
        var isZip = fileName.EndsWith(".zip");

        var endpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.InputFiles;
        endpoint = isZip ? $"{endpoint}.zip" : $"{endpoint}/{fileName}";

        var method = isZip ? Method.Post : Method.Put;

        try
        {
            var response = await Client.UploadFile(endpoint, method, fileParam, Creds);
        }
        catch (PluginApplicationException e)
        {
            throw new PluginApplicationException("Could not upload an input file; " + e.Message);
        }
    }

    protected async Task<List<string>> GetOutputFiles(string projectId)
    {
        var response = await Client.ExecuteWithXml<GetFilesResponse>(ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.OutputFiles, Method.Get, null, Creds);
        return response.FileNames;
    }

    protected async Task DeleteProject(string projectId)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{projectId}", Method.Delete, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new PluginApplicationException($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
    }

    protected async Task<byte[]> DownloadOutputFile(string projectId, string fileName)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.OutputFiles + $"/{fileName}", Method.Get, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new PluginApplicationException($"Status code: {response.StatusCode}, Content: {response.Content}");
        }

        return response.RawBytes;
    }

    protected async Task<Stream> DownloadOutputFileAsStream(string projectId, string fileName)
    {
        var fileBytes = await DownloadOutputFile(projectId, fileName);

        var stream = new MemoryStream(fileBytes);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    protected async Task Execute(string projectId, string? sourceLanguage = null, string? targetLanguage = null, IEnumerable<string>? targetLanguages = null)
    {
        string endpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.Tasks + ApiEndpoints.Execute;
        if (sourceLanguage != null)
        {
            endpoint += $"/{sourceLanguage}";
        }

        if (targetLanguage != null)
        {
            endpoint += $"/{targetLanguage}";
        }
        else if(targetLanguages != null)
        {
            var targetLanguagesQuery = string.Join("&", targetLanguages.Select(lang => $"targets={lang}"));
            endpoint += $"?{targetLanguagesQuery}";
        }

        var response = await Client.Execute(endpoint, Method.Post, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new PluginApplicationException($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
    }
}