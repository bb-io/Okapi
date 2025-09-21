using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi.Models.Responses;

public class ProjectCreatedResponse
{
    [Display("Project ID")]
    public string ProjectId { get; set; } = string.Empty;
}