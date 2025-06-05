using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Responses;

public class PretranslateResponse
{
    [Display("XLIFF file")]
    public FileReference Xliff { get; set; }

    [Display("Package", Description = "Use this package to convert the XLIFF back into its original")]
    public FileReference Package { get; set; }

    [Display("Project ID", Description = "OKAPI Longhorn Project ID for advanced usage and debugging purposes.")]
    public string ProjectId { get; set; }
}
