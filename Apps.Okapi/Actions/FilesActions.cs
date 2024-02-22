using Apps.Okapi.Api;
using RestSharp;
using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.Okapi.Actions;

[ActionList]
public class FilesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Upload files", Description = "Uploads files that will have the names of the files in the request")]
    public async Task UploadFiles([ActionParameter] GetProjectRequest projectRequest, [ActionParameter] UploadFilesRequest request)
    {
        foreach (var file in request.Files)
        {
            var fileBytes = await ValidateAndGetFileBytes(file);
            
            var isZip = file.Name.EndsWith(".zip");
            var endpoint = ConstructEndpoint(projectRequest.ProjectId, file.Name, isZip: isZip);
            
            var method = isZip ? Method.Post : Method.Put;
            await ExecuteUploadRequest(endpoint, fileBytes, file.Name, file.ContentType, method);
        }
    }

    private async Task<byte[]> ValidateAndGetFileBytes(FileReference file)
    {
        if (string.IsNullOrEmpty(file.Name))
        {
            throw new("File name is required.");
        }

        var fileStream = await fileManagementClient.DownloadAsync(file);
        return await fileStream.GetByteData();
    }

    private string ConstructEndpoint(string projectId, string fileName, bool isZip)
    {
        var endpoint = ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.InputFiles;
        return isZip ? $"{endpoint}.zip" : $"{endpoint}/{fileName}";
    }

    private async Task ExecuteUploadRequest(string endpoint, byte[] fileBytes, string fileName, string contentType, Method method)
    {
        var baseUrl = Creds.Get(CredsNames.Url).Value;
        var uploadFileRequest = new OkapiRequest(new OkapiRequestParameters
        {
            Method = method,
            Url = baseUrl + endpoint
        }).AddFile("inputFile", fileBytes, fileName, contentType);

        var response = await Client.ExecuteAsync(uploadFileRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Could not upload your file; Code: {response.StatusCode}; Message: {response.Content}");
        }
    }
}