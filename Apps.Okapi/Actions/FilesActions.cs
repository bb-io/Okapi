using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;

namespace Apps.Okapi.Actions;

[ActionList]
public class FilesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
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
}