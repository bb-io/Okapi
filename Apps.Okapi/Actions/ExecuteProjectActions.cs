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
    [Action("Execute project", Description = "Executes the batch configuration on the uploaded input files, returns the output files")]
    public async Task<ExecuteProjectResponse> ExecuteTasks([ActionParameter] GetProjectRequest projectRequest, [ActionParameter]ExecuteTasksRequest request, [ActionParameter, Display("Delete project")] bool? deleteProject)
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
        
        var delete = deleteProject ?? true;
        if (delete)
        {
            await DeleteProject(projectRequest.ProjectId);
        }
        
        return new ExecuteProjectResponse
        {
            OutputFiles = outputFiles
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
            
            if(request.TargetLanguage == null && request.TargetLanguages == null)
            {
                throw new("Source language is set, but neither target language nor Target languages are set. One of them should be set.");
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
    
    private async Task<List<FileReference>> DownloadAllOutputFiles(string projectId)
    {
        var fileNames = await GetOutputFiles(projectId);
        
        var downloadedFiles = new List<FileReference>();
        foreach (var fileName in fileNames.FileNames)
        {
            var fileReference = await DownloadOutputFile(projectId, fileName);
            downloadedFiles.Add(fileReference);
        }

        return downloadedFiles;
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

        string mimeType = MimeTypes.GetMimeType(fileName);
        return await fileManagementClient.UploadAsync(stream, mimeType, fileName);
    }
    
    private async Task DeleteProject(string projectId)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{projectId}", Method.Delete, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
    }
}