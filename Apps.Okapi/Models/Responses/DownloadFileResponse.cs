using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Responses;

public class DownloadFileResponse()
{
    public FileReference File { get; set; } = new();
}
