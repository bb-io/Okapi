using Apps.Okapi.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Okapi.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups =>
    [
        new()
        {
            Name = "Base URL",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = [
                new ConnectionProperty(CredsNames.Url) { DisplayName = "URL", Description = "Base URL of the Okapi Longhorn server, e.g. http://28.216.252.148:88/okapi-longhorn" }
            ],
        }
    ];

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        var clientIdKeyValue = values.First(v => v.Key == CredsNames.Url);
        yield return new AuthenticationCredentialsProvider( clientIdKeyValue.Key,clientIdKeyValue.Value);
    }
}