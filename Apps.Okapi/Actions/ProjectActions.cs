using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Okapi.Actions;

[ActionList("Projects")]
public class ProjectActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Create project", Description = "Create new project, returns id of created project, and uploads batch configuration file")]
    public async Task<ProjectCreatedResponse> CreateProject([ActionParameter] UploadBatchConfigurationFileRequest request)
    {
        var projectId = await CreateNewProject();

        var fileStream = await fileManagementClient.DownloadAsync(request.File);
        var fileBytes = await fileStream.GetByteData();

        await AddBatchConfig(
            projectId,
            fileBytes,
            request.File.Name,
            request.File.ContentType,
            request.BatchOverwrite);

        return new ProjectCreatedResponse
        {
            ProjectId = projectId
        };
    }

    [Action("Upload files", Description = "Uploads files that will have the names of the files in the request")]
    public async Task UploadFiles([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] UploadFilesRequest request)
    {
        foreach (var file in request.Files)
        {
            var fileStream = await fileManagementClient.DownloadAsync(file);
            var fileBytes = await fileStream.GetByteData();

            await UploadFile(projectRequest.ProjectId, fileBytes, file.Name, file.ContentType);
        }
    }

    [Action("Execute project", Description = "Executes the batch configuration on the uploaded input files, returns the output files")]
    public async Task<ExecuteProjectResponse> ExecuteTasks([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] ExecuteTasksRequest request, [ActionParameter, Display("Delete project")] bool? deleteProject)
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