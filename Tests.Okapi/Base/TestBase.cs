using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Extensions.Configuration;

namespace Tests.Okapi.Base;

public class TestBase
{
    public IEnumerable<AuthenticationCredentialsProvider> Creds { get; private set; }
    public InvocationContext InvocationContext { get; private set; }
    public FileManagementClient FileManagementClient { get; private set; }
    public string LonghornWorkDir { get; private set; }

    public TestBase()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        Creds = config.GetSection("ConnectionDefinition")
                     .GetChildren()
                     .Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value))
                     .ToList();

        InvocationContext = new InvocationContext
        {
            AuthenticationCredentialsProviders = Creds
        };

        FileManagementClient = new FileManagementClient(config.GetSection("TestFolder").Value!);

        LonghornWorkDir = config.GetSection("LonghornWorkDir").Value!;
    }
}
