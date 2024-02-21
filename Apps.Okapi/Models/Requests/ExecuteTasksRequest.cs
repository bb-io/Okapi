using Apps.Okapi.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Okapi.Models.Requests;

public class ExecuteTasksRequest
{
    [Display("Source language"), DataSource(typeof(LanguageDataHandler))]
    public string? SourceLanguage { get; set; }
    
    [Display("Target language"), DataSource(typeof(LanguageDataHandler))]
    public string? TargetLanguage { get; set; }

    [Display("Target languages"), DataSource(typeof(LanguageDataHandler))]
    public IEnumerable<string>? TargetLanguages { get; set; }
}