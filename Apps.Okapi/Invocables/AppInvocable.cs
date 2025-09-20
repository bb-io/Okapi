using Apps.Okapi.Api;
using Apps.Okapi.Constants;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using System;
using System.IO.Compression;
using System.Net;
using System.Reflection;

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

    protected async Task AddBatchConfig(string projectId, byte[] fileBytes, string name = "batch.bconf"
        , string contentType = "application/octet-stream", string? configOverride = null)
    {
        string endpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.BatchConfiguration;
        var fileParam = FileParameter.Create("batchConfiguration", fileBytes, name, contentType);

        try
        {
            List<Parameter> formParams = [];
            if (!string.IsNullOrEmpty(configOverride))
            {
                formParams.Add(Parameter.CreateParameter("overrideStepParams", configOverride, ParameterType.GetOrPost));
            }

            await Client.UploadFile(endpoint, Method.Post, fileParam, Creds, formParams);
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

    protected async Task UploadFile(string projectId, Func<Stream> getFile, string fileName, string contentType)
    {
        if (string.IsNullOrEmpty(fileName)) throw new("File name is required.");

        var fileParam = FileParameter.Create("inputFile", getFile, fileName, contentType);
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
        return response.RawBytes ?? [];
    }

    protected async Task<Stream> DownloadOutputFileAsStream(string projectId, string fileName)
    {
        var fileBytes = await DownloadOutputFile(projectId, fileName);

        var stream = new MemoryStream(fileBytes);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    protected async Task<Stream> DownloadOutputArchiveAsStream(string projectId)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.OutputArchive, Method.Get, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new PluginApplicationException($"Status code: {response.StatusCode}, Content: {response.Content}");
        }

        if (response.RawBytes == null)
        {
            throw new PluginApplicationException("OKAPI server has returned no data for an output archive.");
        }

        var zipStream = new MemoryStream(response.RawBytes);
        zipStream.Seek(0, SeekOrigin.Begin);

        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Update, leaveOpen: true))
        {
            var entriesToMove = archive.Entries
                .Where(e => !string.IsNullOrEmpty(e.Name))
                .Where(e => e.FullName.Count(c => c == '/') >= 1)
                .ToList();

            var topFolders = entriesToMove
                .Select(e => e.FullName.Split('/')[0])
                .Distinct()
                .ToList();

            // move entries from subfolder to one more level
            foreach (var entry in entriesToMove)
            {
                var parts = entry.FullName.Split('/');

                var newPath = string.Join('/', parts.Skip(1));
                if (archive.GetEntry(newPath) != null)
                {
                    throw new PluginApplicationException("Can't remove subfolder from the package archive.");
                }

                var newEntry = archive.CreateEntry(newPath);
                using (var oldEntryStream = entry.Open())
                using (var newEntryStream = newEntry.Open())
                {
                    oldEntryStream.CopyTo(newEntryStream);
                }
                entry.Delete();
            }

            // remove empty directory entries for the top-level folders
            foreach (var folder in topFolders)
            {
                archive.GetEntry(folder + "/")?.Delete();
            }
        }

        zipStream.Seek(0, SeekOrigin.Begin);
        return zipStream;
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

    protected async Task UploadFile(string projectId, FileParameter file)
    {
        var isZip = file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        var inputEndpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.InputFiles;
        var uploadEndpoint = isZip ? $"{inputEndpoint}.zip" : $"{inputEndpoint}/{file.FileName}";
        var uploadMethod = isZip ? Method.Post : Method.Put;

        await Client.UploadFile(uploadEndpoint, uploadMethod, file, Creds);
    }

    protected static async Task<byte[]> LoadBatchConfig(string configName)
    {
        var asm = Assembly.GetExecutingAssembly();
        Stream? stream = asm.GetManifestResourceStream("Apps.Okapi.Batchconfigs." + $"{configName}.bconf")
            ?? throw new PluginApplicationException($"Embedded batch config '{configName}.bconf' not found.");
        return await stream.GetByteData();
    }

    protected async Task<string> CreateProject()
    {
        var newProjectResponse = await Client.Execute(ApiEndpoints.Projects + "/new", Method.Post, null, Creds);

        var locationHeader = (newProjectResponse?.Headers?.FirstOrDefault(h => h.Name?.Equals("Location", StringComparison.OrdinalIgnoreCase) == true)?.Value?.ToString())
            ?? throw new PluginApplicationException("Cound't get a location header for a created project.");

        var projectId = new Uri(locationHeader).Segments.Last()
            ?? throw new PluginApplicationException("Cound't get a created project from the location header value.");

        return projectId;
    }

    protected async Task<string> CreateProjectWithBatchConfig(string configName, string? configOverride = null)
    {
        var projectId = await CreateProject();

        var config = await LoadBatchConfig(configName);
        await AddBatchConfig(projectId, config, configOverride: configOverride);

        return projectId;
    }
}