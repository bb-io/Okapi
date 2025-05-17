using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Requests;

public class UploadTranslationAssetsRequest
{
    [Display("OKAPI Longhorn working directory", Description = "Absolute path to the Okapi working directory, f.e.: /users/john/Okapi-Longhorn-Files/.")]
    public string LonghornWorkDir { get; set; }

    [Display("Translation memory (TMX file)")]
    public FileReference Tmx { get; set; }

    [Display("Segmentation rules (SRX file)", Description = "Upload custom segmentation rules.")]
    public FileReference? Srx { get; set;}
}
