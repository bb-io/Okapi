using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Okapi.Actions;

[ActionList]
public class ProjectActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Get projects", Description = "Get ids of projects")]
    public async Task<GetProjectsResponse> GetProjects()
    {
        return await Client.ExecuteWithXml<GetProjectsResponse>(ApiEndpoints.Projects, Method.Get, null, Creds);
    }
    
    [Action("Delete project", Description = "Delete project by id")]
    public async Task DeleteProject([ActionParameter] string projectId)
    {
        var response = await Client.Execute(ApiEndpoints.Projects + $"/{projectId}", Method.Delete, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
    }
    
    [Action("Create project", Description = "Create new project, returns id of created project")]
    public async Task<ProjectCreatedResponse> CreateProject()
    {
        var response = await Client.Execute(ApiEndpoints.Projects, Method.Post, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
        
        var locationHeader = response.Headers.FirstOrDefault(h => h.Name.Equals("Location", StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
        if (locationHeader == null)
        {
            throw new Exception("Location header is missing in the response.");
        }
        
        var uri = new Uri(locationHeader);
        var projectId = uri.Segments.Last();

        return new ProjectCreatedResponse
        {
            ProjectId = projectId
        };
    }
    
    
}