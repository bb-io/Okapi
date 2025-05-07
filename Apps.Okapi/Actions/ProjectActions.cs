using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;

namespace Apps.Okapi.Actions;

[ActionList]
public class ProjectActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Create project", Description = "Create new project, returns id of created project, and uploads batch configuration file")]
    public async Task<ProjectCreatedResponse> CreateProject([ActionParameter] UploadBatchConfigurationFileRequest request)
    {
        var projectId = await CreateNewProject();

        var fileStream = await fileManagementClient.DownloadAsync(request.File);
        var fileBytes = await fileStream.GetByteData();

        await AddBatchConfig(projectId, fileBytes, request.File.Name, request.File.ContentType);

        return new ProjectCreatedResponse
        {
            ProjectId = projectId
        };
    }
}