using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Okapi.Actions;

[ActionList]
public class OutputFilesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Get output files", Description = "Get ids of output files")]
    public async Task<GetFilesResponse> GetInputFiles([ActionParameter] GetProjectRequest projectRequest)
    {
        return await Client.ExecuteWithXml<GetFilesResponse>(ApiEndpoints.Projects + $"/{projectRequest.ProjectId}" + ApiEndpoints.OutputFiles, Method.Get, null, Creds);
    }
    
    [Action("Download output file", Description = "Download output file by name")]
    public async Task<DownloadFileResponse> DownloadOutputFile([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] GetOutputFileRequest request)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{projectRequest.ProjectId}" + ApiEndpoints.OutputFiles + $"/{request.FileName}", Method.Get, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
        
        var stream = new MemoryStream(response.RawBytes);
        stream.Seek(0, SeekOrigin.Begin);
        
        var fileReference = await fileManagementClient.UploadAsync(stream, ContentType.Binary, request.FileName);
        return new(fileReference);
    }
    
    [Action("Download output files as zip", Description = "Download output files as zip")]
    public async Task<DownloadFileResponse> DownloadOutputFilesAsZip([ActionParameter] GetProjectRequest projectRequest)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{projectRequest.ProjectId}" + ApiEndpoints.OutputFiles + ".zip", Method.Get, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
        
        var stream = new MemoryStream(response.RawBytes);
        stream.Seek(0, SeekOrigin.Begin);
        
        var fileReference = await fileManagementClient.UploadAsync(stream, ContentType.Binary, "output_files.zip");
        return new(fileReference);
    }
}