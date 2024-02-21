using System.Net.Mime;
using Apps.Okapi.Api;
using RestSharp;
using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using ContentType = RestSharp.ContentType;

namespace Apps.Okapi.Actions;

[ActionList]
public class InputFilesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Get input files", Description = "Get ids of input files")]
    public async Task<GetFilesResponse> GetInputFiles([ActionParameter] GetProjectRequest projectRequest)
    {
        return await Client.ExecuteWithXml<GetFilesResponse>(ApiEndpoints.Projects + $"/{projectRequest.ProjectId}" + ApiEndpoints.InputFiles, Method.Get, null, Creds);
    }
    
    [Action("Download input file", Description = "Download input file by id")]
    public async Task<DownloadFileResponse> DownloadInputFile([ActionParameter] GetInputFileRequest request)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{request.ProjectId}" + ApiEndpoints.InputFiles + $"/{request.FileName}", Method.Get, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
        
        var stream = new MemoryStream(response.RawBytes);
        stream.Seek(0, SeekOrigin.Begin);
        
        var fileReference = await fileManagementClient.UploadAsync(stream, ContentType.Binary, request.FileName);
        return new(fileReference);
    }
    
    [Action("Upload input file", Description = "Upload input file")]
    public async Task UploadInputFile([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] UploadFileRequest request)
    {
        var fileBytes = await ValidateAndGetFileBytes(request.File);
        var endpoint = ConstructEndpoint(projectRequest.ProjectId, request.File.Name, isZip: false);
        await ExecuteUploadRequest(endpoint, fileBytes, request.File.Name, request.File.ContentType);
    }

    [Action("Upload input files as ZIP", Description = "Upload input files as ZIP")]
    public async Task UploadInputFilesAsZip([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] UploadFileRequest request)
    {
        var fileBytes = await ValidateAndGetFileBytes(request.File);
        var endpoint = ConstructEndpoint(projectRequest.ProjectId, request.File.Name, isZip: true);
        await ExecuteUploadRequest(endpoint, fileBytes, request.File.Name, request.File.ContentType);
    }

    private async Task<byte[]> ValidateAndGetFileBytes(FileReference file)
    {
        if (string.IsNullOrEmpty(file.Name))
        {
            throw new("File name is required");
        }

        var fileStream = await fileManagementClient.DownloadAsync(file);
        return await fileStream.GetByteData();
    }

    private string ConstructEndpoint(string projectId, string fileName, bool isZip)
    {
        var endpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.InputFiles;
        return isZip ? $"{endpoint}.zip" : $"{endpoint}/{fileName}";
    }

    private async Task ExecuteUploadRequest(string endpoint, byte[] fileBytes, string fileName, string contentType)
    {
        var baseUrl = Creds.Get(CredsNames.Url).Value;
        var uploadFileRequest = new OkapiRequest(new OkapiRequestParameters
        {
            Method = Method.Put,
            Url = baseUrl + endpoint
        }).AddFile("inputFile", fileBytes, fileName, contentType);

        var response = await Client.ExecuteAsync(uploadFileRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Could not upload your file; Code: {response.StatusCode}; Message: {response.Content}");
        }
    }
}