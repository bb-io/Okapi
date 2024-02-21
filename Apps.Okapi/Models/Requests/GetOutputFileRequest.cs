using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi.Models.Requests;

public class GetOutputFileRequest
{
    [Display("File name")]
    public string FileName { get; set; }
}