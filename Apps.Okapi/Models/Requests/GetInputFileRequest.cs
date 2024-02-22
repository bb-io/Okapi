using Apps.Okapi.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Okapi.Models.Requests;

public class GetInputFileRequest
{
    [Display("Project ID"), DataSource(typeof(ProjectDataHandler))]
    public string ProjectId { get; set; }
    
    [Display("File name"), DataSource(typeof(InputFileDataHandler))]
    public string FileName { get; set; }
}