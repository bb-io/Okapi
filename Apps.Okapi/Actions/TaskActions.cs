using RestSharp;
using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Okapi.Actions;

[ActionList]
public class TaskActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Execute tasks", Description = "Executes the Batch Configuration on the uploaded input files")]
    public async Task ExecuteTasks([ActionParameter] GetProjectRequest projectRequest, [ActionParameter]ExecuteTasksRequest request)
    {
        ValidateRequest(request);
        
        string endpoint = ApiEndpoints.Projects + $"/{projectRequest.ProjectId}" + ApiEndpoints.Tasks + ApiEndpoints.Execute;
        if(request.SourceLanguage != null)
        {
            endpoint += $"/{request.SourceLanguage}";
        }
        
        if (request.TargetLanguage != null)
        {
            endpoint += $"/{request.TargetLanguage}";
        }
        else if (request.TargetLanguages != null)
        {
            var targetLanguagesQuery = string.Join("&", request.TargetLanguages.Select(lang => $"targets={lang}"));
            endpoint += $"?{targetLanguagesQuery}";
        }
        
        var response = await Client.Execute(endpoint, Method.Post, null, Creds);
        if (!response.IsSuccessStatusCode)
        {
            throw new($"Status code: {response.StatusCode}, Content: {response.Content}");
        }
    }
    
    private void ValidateRequest(ExecuteTasksRequest request)
    {
        if (request.SourceLanguage != null)
        {
            if (request.TargetLanguage != null && request.TargetLanguages != null)
            {
                throw new("Both Target language and Target languages are set. Only one of them should be set.");
            }
        }
        else
        {
            if (request.TargetLanguage != null || request.TargetLanguages != null)
            {
                throw new("Source language is not set, but Target language or Target languages are set. Source language should be set.");
            }
        }
    }
}