using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Requests;

public class UploadBatchConfigurationFileRequest
{
    [Display("Batch configuration file")]
    public FileReference File { get; set; }
}