using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Okapi.DataSourceHandlers;

public class OutputFileDataHandler : AppInvocable, IAsyncDataSourceHandler
{
    private readonly string _projectId;
    
    public OutputFileDataHandler(InvocationContext invocationContext, [ActionParameter] GetOutputFileRequest projectRequest) : base(invocationContext)
    {
        _projectId = projectRequest.ProjectId;
    }
    
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (_projectId is null)
        {
            throw new("You must input Project Id first");
        }
        
        string endpoint = ApiEndpoints.Projects + $"/{_projectId}" + ApiEndpoints.OutputFiles;
        var files = await Client.ExecuteWithXml<GetFilesResponse>(endpoint, Method.Get, null, Creds);
        
        return files.FileNames
            .Where(x => context.SearchString == null ||
                        x.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToDictionary(x => x, x => x);
    }
}
