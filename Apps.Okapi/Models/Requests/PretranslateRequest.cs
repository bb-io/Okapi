using Apps.Okapi.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Requests;

public class PretranslateRequest
{
    [Display("Source file", Description = "File to be translated (source file).")]
    public FileReference SourceFile { get; set; }

    [Display("Translation memory path (TMX file)", Description = "Path to TM local to OKAPI server, use \"Upload translation assets\" action to obtain this path.")]
    public string TmxPath { get; set; }

    [Display("Segmentation rules path (SRX file)", Description = "Path to custom segmentation rules local to OKAPI server, use \"Upload translation assets\" action to obtain this path.")]
    public string? SrxPath { get; set; }
}
