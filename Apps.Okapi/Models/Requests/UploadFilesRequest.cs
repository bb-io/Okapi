using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Requests;

public class UploadFilesRequest
{
    public IEnumerable<FileReference> Files { get; set; }
}