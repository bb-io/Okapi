namespace Apps.Okapi.Models.Dtos;

public record class TmExportConfigOverrideDto
{
    public required string OutputPath { get; init; }

    public bool UseGenericCodes { get; init; } = false;

    public bool SkipEntriesWithoutText { get; init; } = false;

    public bool ApprovedEntriesOnly { get; init; } = false;

    public bool OverwriteSameSource { get; init; } = true;
}
