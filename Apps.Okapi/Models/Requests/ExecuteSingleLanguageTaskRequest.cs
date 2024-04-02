using Apps.Okapi.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Okapi.Models.Requests
{
    public class ExecuteSingleLanguageTaskRequest
    {
        [Display("Source language"), DataSource(typeof(LanguageDataHandler))]
        public string SourceLanguage { get; set; }

        [Display("Target language"), DataSource(typeof(LanguageDataHandler))]
        public string TargetLanguage { get; set; }
    }
}
