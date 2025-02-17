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
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Okapi.Actions;

[ActionList]
public class ExecuteProjectActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Execute project", Description = "Executes the batch configuration on the uploaded input files, returns the output files")]
    public async Task<ExecuteProjectResponse> ExecuteTasks([ActionParameter] GetProjectRequest projectRequest, [ActionParameter]ExecuteTasksRequest request, [ActionParameter, Display("Delete project")] bool? deleteProject)
    {
        if (request.SourceLanguage != null)
        {
            if (request.TargetLanguage != null && request.TargetLanguages != null)
            {
                throw new PluginMisconfigurationException("Both Target language and Target languages are set. Only one of them should be set.");
            }

            if (request.TargetLanguage == null && request.TargetLanguages == null)
            {
                throw new PluginMisconfigurationException("Source language is set, but neither target language nor Target languages are set. One of them should be set.");
            }
        }
        else
        {
            if (request.TargetLanguage != null || request.TargetLanguages != null)
            {
                throw new PluginMisconfigurationException("Source language is not set, but Target language or Target languages are set. Source language should be set.");
            }
        }

        await Execute(projectRequest.ProjectId, request.SourceLanguage, request.TargetLanguage, request.TargetLanguages);

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
    
    private async Task<List<FileReference>> DownloadAllOutputFiles(string projectId)
    {
        var fileNames = await GetOutputFiles(projectId);
        
        var downloadedFiles = new List<FileReference>();
        foreach (var fileName in fileNames)
        {
            var stream = await DownloadOutputFileAsStream(projectId, fileName);
            string mimeType = MimeTypes.GetMimeType(fileName);
            var fileReference = await fileManagementClient.UploadAsync(stream, mimeType, fileName);

            downloadedFiles.Add(fileReference);
        }

        return downloadedFiles;
    }   

}