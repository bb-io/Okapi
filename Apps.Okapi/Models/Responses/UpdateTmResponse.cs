using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi.Models.Responses;

public class UpdateTmResponse
{
    [Display("Segments sent to TM")]
    public int SegmentsSent { get; set; }

    [Display("Total segments in file")]
    public int TotalSegmentsInFile { get; set; }
}