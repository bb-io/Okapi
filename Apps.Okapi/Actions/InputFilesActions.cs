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
    [Action("Get input file IDs", Description = "Get ids of input files")]
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
    
    [Action("Download all input files", Description = "Download all input files from the project")]
    public async Task<DownloadFilesResponse> DownloadAllInputFiles([ActionParameter] GetProjectRequest projectRequest)
    {
        var fileNames = await GetInputFiles(projectRequest);
        
        var downloadFilesResponse = new DownloadFilesResponse();
        foreach (var fileName in fileNames.FileNames)
        {
            var fileReference = await DownloadInputFile(new GetInputFileRequest
            {
                ProjectId = projectRequest.ProjectId,
                FileName = fileName
            });
            
            downloadFilesResponse.Files.Add(fileReference.File);
        }
        
        return downloadFilesResponse;
    }
    
    [Action("Upload input file", Description = "Uploads a file that will have the name of the file in the request")]
    public async Task UploadInputFile([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] UploadFileRequest request)
    {
        var fileBytes = await ValidateAndGetFileBytes(request.File);
        var endpoint = ConstructEndpoint(projectRequest.ProjectId, request.File.Name, isZip: false);
        await ExecuteUploadRequest(endpoint, fileBytes, request.File.Name, request.File.ContentType, Method.Put);
    }

    [Action("Upload input files as ZIP", Description = "Adds input files as a zip archive (the zip will be extracted and the included files will be used as input files)")]
    public async Task UploadInputFilesAsZip([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] UploadFileRequest request)
    {
        var fileBytes = await ValidateAndGetFileBytes(request.File);
        var endpoint = ConstructEndpoint(projectRequest.ProjectId, request.File.Name, isZip: true);
        await ExecuteUploadRequest(endpoint, fileBytes, request.File.Name, request.File.ContentType, Method.Post);
    }

    private async Task<byte[]> ValidateAndGetFileBytes(FileReference file)
    {
        if (string.IsNullOrEmpty(file.Name))
        {
            throw new("File name is required.");
        }

        var fileStream = await fileManagementClient.DownloadAsync(file);
        return await fileStream.GetByteData();
    }

    private string ConstructEndpoint(string projectId, string fileName, bool isZip)
    {
        var endpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.InputFiles;
        return isZip ? $"{endpoint}.zip" : $"{endpoint}/{fileName}";
    }

    private async Task ExecuteUploadRequest(string endpoint, byte[] fileBytes, string fileName, string contentType, Method method)
    {
        var baseUrl = Creds.Get(CredsNames.Url).Value;
        var uploadFileRequest = new OkapiRequest(new OkapiRequestParameters
        {
            Method = method,
            Url = baseUrl + endpoint
        }).AddFile("inputFile", fileBytes, fileName, contentType);

        var response = await Client.ExecuteAsync(uploadFileRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Could not upload your file; Code: {response.StatusCode}; Message: {response.Content}");
        }
    }
}