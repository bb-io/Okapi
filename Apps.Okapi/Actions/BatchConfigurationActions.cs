using Apps.Okapi.Api;
using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Okapi.Actions;

[ActionList]
public class BatchConfigurationActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Upload batch configuration", Description = "Upload batch configuration file")]
    public async Task UploadBatchConfiguration([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] UploadFileRequest request)
    {
        string endpoint = ApiEndpoints.Projects + $"/{projectRequest.ProjectId}" + ApiEndpoints.BatchConfiguration;

        var fileStream = await fileManagementClient.DownloadAsync(request.File);
        var fileBytes = await fileStream.GetByteData();
        
        var baseUrl = Creds.Get(CredsNames.Url).Value;
        var uploadFileRequest = new OkapiRequest(new OkapiRequestParameters
        {
            Method = Method.Post,
            Url = baseUrl + endpoint
        }).AddFile("batchConfiguration", fileBytes, request.File.Name, request.File.ContentType);

        var response = await Client.ExecuteAsync(uploadFileRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Could not upload your file; Code: {response.StatusCode}; Message: {response.Content}");
        }
    }
}