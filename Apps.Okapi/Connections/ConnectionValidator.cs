using Apps.Okapi.Api;
using Apps.Okapi.Constants;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Okapi.Connections;

public class ConnectionValidator : IConnectionValidator
{    
    private readonly OkapiClient _client = new();

    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            await _client.ExecuteWithXml<GetProjectsResponse>(ApiEndpoints.Projects, Method.Get, null,
                authenticationCredentialsProviders.ToArray());
            
            return new() { IsValid = true };
        }
        catch (Exception e)
        {
            return new ConnectionValidationResponse { IsValid = false, Message = e.Message };
        }
    }
}