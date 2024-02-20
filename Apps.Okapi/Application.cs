using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi;

public class Application : IApplication
{
    public string Name
    {
        get => "App";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}