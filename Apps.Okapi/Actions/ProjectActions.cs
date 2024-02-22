using RestSharp;
using Apps.Okapi.Api;
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

namespace Apps.Okapi.Actions;

[ActionList]
public class ProjectActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Create project", Description = "Create new project, returns id of created project, and uploads batch configuration file")]
    public async Task<ProjectCreatedResponse> CreateProject([ActionParameter] UploadBatchConfigurationFileRequest request)
    {
        var response = await Client.Execute(ApiEndpoints.Projects, Method.Post, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
        
        var locationHeader = response.Headers.FirstOrDefault(h => h.Name.Equals("Location", StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
        if (locationHeader == null)
        {
            throw new Exception("Location header is missing in the response.");
        }
        
        var uri = new Uri(locationHeader);
        var projectId = uri.Segments.Last();

        var projectCreatedResponse = new ProjectCreatedResponse
        {
            ProjectId = projectId
        };
        
        await UploadBatchConfiguration(projectId, request.File);
        return projectCreatedResponse;
    }
    
    private async Task UploadBatchConfiguration(string projectId, FileReference file)
    {
        string endpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.BatchConfiguration;

        var fileStream = await fileManagementClient.DownloadAsync(file);
        var fileBytes = await fileStream.GetByteData();
        
        var baseUrl = Creds.Get(CredsNames.Url).Value;
        var uploadFileRequest = new OkapiRequest(new OkapiRequestParameters
        {
            Method = Method.Post,
            Url = baseUrl + endpoint
        }).AddFile("batchConfiguration", fileBytes, file.Name, file.ContentType);

        var response = await Client.ExecuteAsync(uploadFileRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Could not upload your batch configuration file; Code: {response.StatusCode}; Message: {response.Content}");
        }
    }
}