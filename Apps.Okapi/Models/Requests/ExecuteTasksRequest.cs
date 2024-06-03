using Apps.Okapi.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Okapi.Models.Requests;

public class ExecuteTasksRequest
{
    [Display("Source language"), StaticDataSource(typeof(LanguageDataHandler))]
    public string? SourceLanguage { get; set; }
    
    [Display("Target language"), StaticDataSource(typeof(LanguageDataHandler))]
    public string? TargetLanguage { get; set; }

    [Display("Target languages"), StaticDataSource(typeof(LanguageDataHandler))]
    public IEnumerable<string>? TargetLanguages { get; set; }

}