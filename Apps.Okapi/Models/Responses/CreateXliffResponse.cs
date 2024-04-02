using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Okapi.Models.Responses
{
    public class CreateXliffResponse
    {
        [Display("XLIFF file")]
        public FileReference Xliff { get; set; }

        [Display("Package", Description = "Use this package to convert the XLIFF back into its original")]
        public FileReference Package { get; set;}
    }
}
