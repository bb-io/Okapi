using Apps.Okapi.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Okapi.Models.Requests;

public class ExportTmxRequest
{
    [Display("Absolute local path to TM", Description = "Absolute path to the translation memory on the OKAPI Longhorn server.")]
    public string TmPath { get; set; } = string.Empty;
}