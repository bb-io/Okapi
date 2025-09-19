using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.Okapi.DataSourceHandlers.EnumHandlers;

namespace Apps.Okapi.Models.Requests;

public class UpdateTmRequest
{
    [Display("Bilingual file", Description = "Upload bilingual content (e.g., XLIFF/TMX) to update the translation memory.")]
    public FileReference BilingualFile { get; set; } = new();

    [Display("Translation memory path", Description = "Absolute path to the translation memory on the OKAPI Longhorn server.")]
    public string TMPath { get; set; } = string.Empty;

    [Display("Segment state to import", Description = "State of the segments to be imported into the translation memory, by default all existing translations will be sent to TM.")]
    [StaticDataSource(typeof(XliffStateDataSourceHandler))]
    public IEnumerable<string>? SegmentStates { get; set; }

    [Display("Exclude empty segments (true by default)", Description = "Skip segments that don't have source or target cotnent.")]
    public bool? ExcludeEmpty { get; set; } = true;

    [Display("Overwrite same source (true by default)", Description = "Overwrite TM entries even if the source language matches, so there would one translation per source.")]
    public bool? OverwriteSameSource { get; set; }
}