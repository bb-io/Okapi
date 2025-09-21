using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Responses
{
    public class CreateXliffResponse
    {
        [Display("XLIFF file")]
        public FileReference Xliff { get; set; } = new();

        [Display("Package", Description = "Use this package to convert the XLIFF back into its original")]
        public FileReference Package { get; set;} = new();
    }
}
