using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi.Models.Responses;

public class CreateTmResponse
{
    [Display("Absolute local path to TM", Description = "Absolute local path to the translation memory on the OKAPI Longhorn server.")]
    public string TmPath { get; set; } = string.Empty;
}