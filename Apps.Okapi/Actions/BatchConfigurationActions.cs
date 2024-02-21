using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Okapi.Actions;

[ActionList]
public class BatchConfigurationActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Upload batch configuration", Description = "Upload batch configuration file")]
    public async Task UploadBatchConfiguration([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] UploadFileRequest request)
    {
        string endpoint = ApiEndpoints.Projects + $"/{projectRequest.ProjectId}" + ApiEndpoints.BatchConfiguration;
        var response = await Client.Execute( endpoint, Method.Post, request.File, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
    }
}