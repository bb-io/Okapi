using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi.Models.Responses;

public class UploadTranslationAssetsResponse
{
    [Display("Translation memory (TMX)")]
    public string TMX { get; set; }

    [Display("Translation memory (TMX)")]
    public string? SRX { get; set; }
}
