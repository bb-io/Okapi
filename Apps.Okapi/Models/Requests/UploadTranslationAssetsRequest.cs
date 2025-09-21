using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Requests;

public class UploadTranslationAssetsRequest
{
    [Display("OKAPI Longhorn working directory", Description = "Absolute path to the Okapi working directory, f.e.: /users/john/Okapi-Longhorn-Files/.")]
    public string LonghornWorkDir { get; set; } = string.Empty;

    [Display("Translation memory (TMX file)", Description = "Use 'Create TM from TMX' for TMs to improve performance and be able to add new segments to an existing TM.")]
    public FileReference? Tmx { get; set; }

    [Display("Segmentation rules (SRX file)", Description = "Upload custom segmentation rules.")]
    public FileReference? Srx { get; set;}
}
