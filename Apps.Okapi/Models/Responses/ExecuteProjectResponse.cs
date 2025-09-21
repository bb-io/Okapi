using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Responses;

public class ExecuteProjectResponse
{
    [Display("Output files")]
    public IEnumerable<FileReference> OutputFiles { get; set; } = [];
}