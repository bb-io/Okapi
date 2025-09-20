using Apps.Okapi.Constants;
using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Apps.Okapi.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using RestSharp;

namespace Apps.Okapi.Actions;

[ActionList]
public class TmActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Create TM from TMX", Description = "Import TMX file to create or update a translation memory.")]
    public async Task<CreateTmResponse> CreateTm([ActionParameter] CreateTmRequest request)
    {
        var projectId = await CreateProject();

        // It's important to store TM in project's output folder to be able to run export later.
        var separatorUsed = request.LonghordWorkDir.Contains('\\') ? '\\' : '/';
        var storagePath = string.Join(separatorUsed, request.LonghordWorkDir.TrimEnd(separatorUsed), projectId, "output", "tm.pentm");
        var importConfigOverwrite = BatchConfig.OverrideTmImportConfig(storagePath, request.OverwriteSameSource == true);

        var importConfig = await LoadBatchConfig("import_to_tm");
        await AddBatchConfig(projectId, importConfig, configOverride: importConfigOverwrite);

        // Upload TMX file by streaming from file management client to avoid loading whole file into memory.
        // RestSharp's FileParameter supports a Func<Stream> to provide a stream at request time.
        // We provide a delegate that opens the download stream synchronously (blocking) so that the
        // actual bytes are streamed directly from the source to the HTTP request body.
        var tmxStream = await fileManagementClient.DownloadAsync(request.Tmx)
            ?? throw new PluginApplicationException("Can't download TMX file.");

        FileParameter tmxFileParam = FileParameter.Create(
            "inputFile",
            () => tmxStream,
            request.Tmx.Name,
            request.Tmx.ContentType
        );

        await UploadFile(projectId, tmxFileParam);
        await Execute(projectId, request.SourceLanguage, targetLanguages: request.TargetLanguages);

        return new CreateTmResponse
        {
            TmPath = storagePath
        };
    }

    [Action("Download Pensieve TM", Description = "Export a translation memory content as archived Pensieve TM (could be locally exported to a TMX using OKAPI).")]
    public Task<ExportTmxResponse> DownloadTm([ActionParameter] ExportTmxRequest request)
    {
        // TM Path is expected to be located in output DIR
        // Pensieve TM can't be convered using OKAPI Longhorn as Longhord don't work with folders as inputs.
        // So users need to download Pnesieve TM , convert it to TMX using OKAPI Rainbow or Tikal

        var separatorUsed = request.TmPath.Contains('\\') ? '\\' : '/';
        var tmPathParts = request.TmPath.Split(separatorUsed, StringSplitOptions.RemoveEmptyEntries);

        if (tmPathParts.Length == 0 || tmPathParts[0].StartsWith("http"))
            throw new PluginApplicationException("TM path must be a local path.");

        if (!tmPathParts[^1].EndsWith(".pentm") || tmPathParts[^2] != "output")
            throw new PluginApplicationException("TM path must point to a TM folder ending with '.pentm' located in OKAPI's project 'output'.");

        var projectId = tmPathParts[^3];

        var baseUrl = Creds.FirstOrDefault(c => c.KeyName == CredsNames.Url)?.Value.TrimEnd('/')
            ?? throw new PluginApplicationException("Can't get a base URL for an output file.");

        var zipRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(baseUrl + ApiEndpoints.Projects + $"/{projectId}" + ApiEndpoints.OutputArchive),
            Method = HttpMethod.Get
        };

        return Task.FromResult(new ExportTmxResponse
        {
            TmxFile = new FileReference(zipRequest, "tm-export.zip", "application/zip"),
        });
    }

    [Action("Update TM", Description = "Add a bilingual file content to a translation memory.")]
    public async Task<UpdateTmResponse> UpdateTm([ActionParameter] UpdateTmRequest request)
    {
        var longhornFileSeparator = request.TmPath.Contains('\\') ? '\\' : '/';
        var tmPathParts = request.TmPath.Split(longhornFileSeparator);

        if (tmPathParts.Length == 0)
            throw new PluginMisconfigurationException("Can't parse path to TM.");

        if (!tmPathParts[^1].EndsWith(".pentm"))
            throw new PluginMisconfigurationException("Path must point at directory and its name must end with '.pentm'.");

        if (tmPathParts[^2] != "output")
            throw new PluginMisconfigurationException("Path must point at directory under OKAPI Longhorn project's output folder.");

        var projectId = tmPathParts[^3];

        var bilingualFileStream = await fileManagementClient.DownloadAsync(request.BilingualFile)
            ?? throw new PluginApplicationException("Can't download bilingual file.");
        var transformation = await Transformation.Parse(bilingualFileStream, request.BilingualFile.Name);

        if (transformation.TargetLanguage is null || transformation.SourceLanguage is null)
            throw new PluginApplicationException("The provided file is not bilingual XLIFF.");

        var sourceLanguage = transformation.SourceLanguage;
        var targetLanguage = transformation.TargetLanguage;
        var totalSegmentsInFile = transformation.GetSegments().Count();

        var segmentStates = request.SegmentStates?
            .Select(SegmentStateHelper.ToSegmentState)
            .Where(state => state is not null)
            .Select(s => s!.Value);

        TransformationFilter.FilterContent(
            transformation,
            request.ExcludeEmpty != false,
            segmentStates ?? []);

        var xliff20 = Xliff2Serializer.Serialize(transformation, Xliff2Version.Xliff20);
        var xliffBytes = System.Text.Encoding.UTF8.GetBytes(xliff20);

        var uniqueFileName = Path.GetFileNameWithoutExtension(request.BilingualFile.Name) + "_" + Guid.NewGuid() + Path.GetExtension(request.BilingualFile.Name);

        var importConfigOverride = BatchConfig.OverrideTmImportConfig(request.TmPath, request.OverwriteSameSource != false);

        try
        {
            var importConfig = await LoadBatchConfig("import_to_tm");
            await AddBatchConfig(projectId, importConfig, configOverride: importConfigOverride);
            await UploadFile(projectId, FileParameter.Create("inputFile", xliffBytes, uniqueFileName, "application/xliff+xml"));
            await Execute(projectId, sourceLanguage, targetLanguage);
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException($"OKAPI Longhorn responded with error: {ex.Message}", ex);
        }

        return new UpdateTmResponse
        {
            TotalSegmentsInFile = totalSegmentsInFile,
            SegmentsSent = transformation.GetSegments().Count()
        };
    }
}
