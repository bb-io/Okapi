using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Okapi.DataSourceHandlers.EnumHandlers;

public class XliffStateDataSourceHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new("initial", "Initial"),
            new("translated", "Translated"),
            new("reviewed", "Reviewed"),
            new("final", "Final"),
        };
    }
}
