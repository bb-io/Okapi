using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Requests;

public class PretranslateRequest
{
    [Display("Content", Description = "File to be translated (source file).")]
    public FileReference File { get; set; } = new();

    [Display("TM path on OKAPI server to TMX file or Pensieve TM folder)", Description = "Path to TM local to OKAPI server, use \"Create TM from TMX\" or \"Upload translation assets\" action to obtain this path.")]
    public string TmPath { get; set; } = string.Empty;

    [Display("Segmentation rules path on OKAPI server (SRX file)", Description = "Path to custom segmentation rules local to OKAPI server, use \"Upload translation assets\" action to obtain this path.")]
    public string? SrxPath { get; set; }
}
