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

namespace Apps.Okapi.Actions;

[ActionList]
public class ExecuteProjectActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Execute project", Description = "Executes the Batch Configuration on the uploaded input files")]
    public async Task<ExecuteProjectResponse> ExecuteTasks([ActionParameter] GetProjectRequest projectRequest, [ActionParameter]ExecuteTasksRequest request)
    {
        ValidateRequest(request);
        
        string endpoint = ApiEndpoints.Projects + $"/{projectRequest.ProjectId}" + ApiEndpoints.Tasks + ApiEndpoints.Execute;
        if(request.SourceLanguage != null)
        {
            endpoint += $"/{request.SourceLanguage}";
        }
        
        if (request.TargetLanguage != null)
        {
            endpoint += $"/{request.TargetLanguage}";
        }
        else if (request.TargetLanguages != null)
        {
            var targetLanguagesQuery = string.Join("&", request.TargetLanguages.Select(lang => $"targets={lang}"));
            endpoint += $"?{targetLanguagesQuery}";
        }
        
        var response = await Client.Execute(endpoint, Method.Post, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
        
        var outputFiles = await DownloadAllOutputFiles(projectRequest.ProjectId);

        await DeleteProject(projectRequest);
        return new ExecuteProjectResponse()
        {
            OutputFiles = outputFiles.Files
        };
    }
    
    private void ValidateRequest(ExecuteTasksRequest request)
    {
        if (request.SourceLanguage != null)
        {
            if (request.TargetLanguage != null && request.TargetLanguages != null)
            {
                throw new("Both Target language and Target languages are set. Only one of them should be set.");
            }
        }
        else
        {
            if (request.TargetLanguage != null || request.TargetLanguages != null)
            {
                throw new("Source language is not set, but Target language or Target languages are set. Source language should be set.");
            }
        }
    }
    
    private async Task<DownloadFilesResponse> DownloadAllOutputFiles(string projectId)
    {
        var fileNames = await GetOutputFiles(projectId);
        
        var downloadFilesResponse = new DownloadFilesResponse();
        foreach (var fileName in fileNames.FileNames)
        {
            var fileReference = await DownloadOutputFile(projectId, fileName);
            downloadFilesResponse.Files.Add(fileReference);
        }
        
        return downloadFilesResponse;
    }
    
    private async Task<GetFilesResponse> GetOutputFiles(string projectId)
    {
        return await Client.ExecuteWithXml<GetFilesResponse>(ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.OutputFiles, Method.Get, null, Creds);
    }
    
    private async Task<FileReference> DownloadOutputFile(string projectId, string fileName)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.OutputFiles + $"/{fileName}", Method.Get, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
        
        var stream = new MemoryStream(response.RawBytes);
        stream.Seek(0, SeekOrigin.Begin);
        
        return await fileManagementClient.UploadAsync(stream, ContentType.Binary, fileName);
    }
    
    private async Task DeleteProject([ActionParameter] GetProjectRequest request)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{request.ProjectId}", Method.Delete, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
    }
}