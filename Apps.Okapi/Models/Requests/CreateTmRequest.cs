using Apps.Okapi.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Models.Requests;

public class CreateTmRequest
{
    [Display("TMX file (supports zip)")]
    public FileReference Tmx { get; set; } = new();

    [Display("Path to Longhorn work directory", Description = "Absolute local path to OKAPI Longhorn working directory on server.")]
    public string LonghordWorkDir { get; set; } = string.Empty;

    [Display("Source language")]
    [StaticDataSource(typeof(LanguageDataHandler))]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target language")]
    [StaticDataSource(typeof(LanguageDataHandler))]
    public IEnumerable<string> TargetLanguages { get; set; } = [];

    [Display("Overwrite same source (true by default)", Description = "If true, the segments will be overwritten even if the source language matches.")]
    public bool? OverwriteSameSource { get; set; }
}