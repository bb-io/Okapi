using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Okapi.Actions;

[ActionList]
public class ProjectActions : AppInvocable
{
    public ProjectActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get projects", Description = "Get ids of projects")]
    public async Task<GetProjectsResponse> GetProjects()
    {
        return await Client.ExecuteWithXml<GetProjectsResponse>(ApiEndpoints.Projects, Method.Get, null, Creds);
    }
}