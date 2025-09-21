using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Requests;

public class UploadFileRequest
{
    public FileReference File { get; set; } = new();
}