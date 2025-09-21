using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Responses;

public class ExportTmxResponse
{
    [Display("Archived Pensieve TM folder")]
    public FileReference TmxFile { get; set; } = new();
}