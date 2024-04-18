using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi.Models.Requests;

public class FileConversionRequest
{
    [Display("Remove inappropriate characters in file name", Description = @"F.e. replace backslashes '\' with braces '{s}'. By default, this is set to true.")]
    public bool? RemoveInappropriateCharactersInFileName { get; set; }
}