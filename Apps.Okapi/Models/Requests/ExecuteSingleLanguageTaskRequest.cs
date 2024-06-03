using Apps.Okapi.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
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
        [Display("Source language"), StaticDataSource(typeof(LanguageDataHandler))]
        public string SourceLanguage { get; set; }

        [Display("Target language"), StaticDataSource(typeof(LanguageDataHandler))]
        public string TargetLanguage { get; set; }
    }
}
