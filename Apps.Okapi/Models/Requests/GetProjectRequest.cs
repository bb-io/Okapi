using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi.Models.Requests;

public class GetProjectRequest
{
    [Display("Project ID")]
    public string ProjectId { get; set; }
}