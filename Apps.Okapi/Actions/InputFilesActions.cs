using RestSharp;
using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

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
}