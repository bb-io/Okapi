using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Responses;

public class DownloadFilesResponse
{
    public List<FileReference> Files { get; set; } = new();
}