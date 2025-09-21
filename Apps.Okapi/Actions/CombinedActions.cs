using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Apps.Okapi.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using System.IO.Compression;
using System.Text;

namespace Apps.Okapi.Actions;

[ActionList("File conversion")]
public class CombinedActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Convert file to XLIFF", Description = "Convert any Okapi-compatible file format into an XLIFF file.")]
    public async Task<CreateXliffResponse> ConvertFileToXliff([ActionParameter] UploadFileRequest fileRequest,
        [ActionParameter] ExecuteSingleLanguageTaskRequest request,
        [ActionParameter] FileConversionRequest conversionRequest)
    {
        var projectId = await CreateNewProject();

        var xliffCreation = await LoadBatchConfig("xliff_creation");
        await AddBatchConfig(projectId, xliffCreation);

        var fileStream = await fileManagementClient.DownloadAsync(fileRequest.File);
        await UploadFile(projectId, () => fileStream, fileRequest.File.GetFileName(conversionRequest.RemoveInappropriateCharactersInFileName ?? true), fileRequest.File.ContentType);

        await Execute(projectId, request.SourceLanguage, request.TargetLanguage);

        var outputFiles = await GetOutputFiles(projectId);

        var xliff = outputFiles.Find(x => x.Contains("work/"));

        if (xliff == null) throw new PluginApplicationException("No XLIFF file was created by Okapi. Please try again");

        // XLIFF file reference
        var xliffStream = await DownloadOutputFileAsStream(projectId, xliff);
        var xliffFileReference =
            await fileManagementClient.UploadAsync(xliffStream, MimeTypes.GetMimeType(xliff), xliff.Substring(xliff.LastIndexOf('/') + 1));

        // Package file reference
        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileName in outputFiles)
                {
                    var currentFile = archive.CreateEntry(fileName.Remove(0, 6));

                    using var entryStream = currentFile.Open();
                    using (var downloadedFileStream = await DownloadOutputFileAsStream(projectId, fileName))
                    {
                        await downloadedFileStream.CopyToAsync(entryStream);
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            var packageFileReference =
                await fileManagementClient.UploadAsync(memoryStream, MimeTypes.GetMimeType(".zip"), "package.zip");

            await DeleteProject(projectId);

            return new CreateXliffResponse
            {
                Xliff = xliffFileReference,
                Package = packageFileReference,
            };
        }
    }

    [Action("Convert XLIFF to file",
        Description = "Convert any XLIFF file after operations back to its original. Use in conjunctions with 'Convert file to XLIFF'")]
    public async Task<DownloadFileResponse> ConvertXliffToFile([ActionParameter] CreateXliffResponse input)
    {
        var projectId = await CreateNewProject();

        var xliffCreation = await LoadBatchConfig("xliff_merging");
        await AddBatchConfig(projectId, xliffCreation);

        using var xliffStream = await fileManagementClient.DownloadAsync(input.Xliff);
        var seekableXliffStream = new MemoryStream();
        await xliffStream.CopyToAsync(seekableXliffStream);
        var xliffContent = Encoding.UTF8.GetString(seekableXliffStream.ToArray());

        var supportedXliffVersions = new[] { "1.2", "2.0" };
        if (Xliff2Serializer.TryGetXliffVersion(xliffContent, out var xliffVersion)
            && !supportedXliffVersions.Contains(xliffVersion) )
        {
            var transformation = Transformation.Parse(xliffContent, input.Xliff.Name);
            var xliff20 = Xliff2Serializer.Serialize(transformation, Xliff2Version.Xliff20);
            await seekableXliffStream.DisposeAsync();
            seekableXliffStream = new MemoryStream(Encoding.UTF8.GetBytes(xliff20));
        }

        using var packageStream = await fileManagementClient.DownloadAsync(input.Package);

        using var memoryStream = new MemoryStream();

        await packageStream.CopyToAsync(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Update, leaveOpen: true))
        {
            var xliffEntry = archive.Entries.FirstOrDefault(x => x.Name.Contains(".xlf")) ?? throw new PluginApplicationException("The package did not contain any XLIFF files");
            xliffEntry.Delete();
            xliffEntry = archive.CreateEntry(xliffEntry.Name);

            using var entryStream = xliffEntry.Open();
            seekableXliffStream.Seek(0, SeekOrigin.Begin);
            await seekableXliffStream.CopyToAsync(entryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        await UploadFile(projectId, () => memoryStream, input.Package.Name, input.Package.ContentType);
        
        await Execute(projectId);

        var outputFiles = await GetOutputFiles(projectId);

        if (outputFiles.Count == 0)
            throw new PluginApplicationException("No files were converted");

        var outputFileName = outputFiles.First();

        var stream = await DownloadOutputFileAsStream(projectId, outputFileName);
        var mimeType = MimeTypes.GetMimeType(outputFileName);
        if (outputFileName.EndsWith(".html"))
        {
            stream = ReplaceInappropriateStrings(stream);
        }

        outputFileName = FileReferenceExtensions.RestoreInappropriateCharacters(outputFileName);

        var fileReference = await fileManagementClient.UploadAsync(stream, mimeType, FileReferenceExtensions.RestoreInappropriateCharacters(outputFileName));

        await DeleteProject(projectId);

        return new DownloadFileResponse() { File = fileReference };
    }

    private static MemoryStream ReplaceInappropriateStrings(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        
        using var reader = new StreamReader(memoryStream);
        var content = reader.ReadToEnd();
        
        var inappropriateStrings = new Dictionary<string, string>
        {
            { "&lt;", "<" },
            { "&gt;", ">" },
        };
        
        foreach (var (key, value) in inappropriateStrings)
        {
            content = content.Replace(key, value);
        }
        
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        ms.Seek(0, SeekOrigin.Begin);
        
        return ms;
    }
}