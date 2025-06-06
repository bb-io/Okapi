﻿using Apps.Okapi.Invocables;
using Apps.Okapi.Models.Requests;
using Apps.Okapi.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Apps.Okapi.Utils;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Okapi.Actions;

[ActionList]
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
        var fileBytes = await fileStream.GetByteData();
        await UploadFile(projectId, fileBytes, fileRequest.File.GetFileName(conversionRequest.RemoveInappropriateCharactersInFileName ?? true), fileRequest.File.ContentType);

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
        Description =
            "Convert any XLIFF file after operations back to its original. Use in conjunctions with 'Convert file to XLIFF'")]
    public async Task<DownloadFileResponse> ConvertXliffToFile([ActionParameter] CreateXliffResponse input)
    {
        var projectId = await CreateNewProject();

        var xliffCreation = await LoadBatchConfig("xliff_merging");
        await AddBatchConfig(projectId, xliffCreation);

        using var xliffStream = await fileManagementClient.DownloadAsync(input.Xliff);
        using var packageStream = await fileManagementClient.DownloadAsync(input.Package);
        using (var memoryStream = new MemoryStream())
        {
            await packageStream.CopyToAsync(memoryStream);
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Update))
            {
                var xliffEntry = archive.Entries.FirstOrDefault(x => x.Name.Contains(".xlf"));
                if (xliffEntry == null) throw new PluginApplicationException("The package did not contain any XLIFF files");
                xliffEntry.Delete();
                xliffEntry = archive.CreateEntry($"work/{xliffEntry.Name}");

                using var entryStream = xliffEntry.Open();
                await xliffStream.CopyToAsync(entryStream);
            }

            var fileBytes = memoryStream.ToArray();

            await UploadFile(projectId, fileBytes, input.Package.Name, input.Package.ContentType);

            await Execute(projectId);

            var outputFiles = await GetOutputFiles(projectId);
            if (outputFiles.Count == 0) throw new PluginApplicationException("No files were converted");
            var outputFile = outputFiles.First();

            var stream = await DownloadOutputFileAsStream(projectId, outputFile);
            var mimeType = MimeTypes.GetMimeType(outputFile);
            if (outputFile.EndsWith(".html"))
            {
                stream = ReplaceInappropriateStrings(stream);
            }

            outputFile = FileReferenceExtensions.RestoreInappropriateCharacters(outputFile);
            
            var fileReference = await fileManagementClient.UploadAsync(stream, mimeType, FileReferenceExtensions.RestoreInappropriateCharacters(outputFile));

            await DeleteProject(projectId);

            return new DownloadFileResponse(fileReference);
        }
    }

    [Action("Upload translation assets", Description = "Uploads TMX and optional SRX files to a new Okapi project and returns the names of the uploaded files.")]
    public async Task<UploadTranslationAssetsResponse> UploadTranslationAssets([ActionParameter] UploadTranslationAssetsRequest request)
    {
        var projectId = await CreateNewProject();
        var longhornFileSeparator = request.LonghornWorkDir.Contains('\\') ? '\\' : '/';
        var longhornWorkDir = request.LonghornWorkDir.TrimEnd(longhornFileSeparator);
        var projectInputDir = string.Join(longhornFileSeparator, longhornWorkDir, projectId, "input");

        var tmxStream = await fileManagementClient.DownloadAsync(request.Tmx);
        var tmxBytes = await tmxStream.GetByteData();
        await UploadFile(projectId, tmxBytes, request.Tmx.Name, request.Tmx.ContentType);
        var response = new UploadTranslationAssetsResponse
        {
            TMX = string.Join(longhornFileSeparator, projectInputDir, request.Tmx.Name)
        };

        if (request.Srx != null)
        {
            var srxStream = await fileManagementClient.DownloadAsync(request.Srx);
            var srxBytes = await srxStream.GetByteData();
            await UploadFile(projectId, srxBytes, request.Srx.Name, request.Srx.ContentType);
            response.SRX = string.Join(longhornFileSeparator, projectInputDir, request.Srx.Name);
        }

        return response;
    }

    [Action("Pre-translate with previous translations",
        Description = "Parse and segment incoming files, then insert previous translations from translation memory. Returns XLIFF v2.1.")]
    public async Task<PretranslateResponse> Pretranslate(
        [ActionParameter] PretranslateRequest pretranslateRequest,
        [ActionParameter] ExecuteSingleLanguageTaskRequest languageTaskRequest,
        [ActionParameter] FileConversionRequest conversionRequest)
    {
        var projectId = await CreateNewProject();

        var pretranslationConfig = await LoadBatchConfig("pretranslation");
        var configOverride = BatchConfig.OverridePretranslateConfig(pretranslateRequest.TmxPath, pretranslateRequest.SrxPath);
        await AddBatchConfig(projectId, pretranslationConfig, configOverride: configOverride);

        var fileStream = await fileManagementClient.DownloadAsync(pretranslateRequest.SourceFile);
        var fileBytes = await fileStream.GetByteData();
        var fileName = pretranslateRequest.SourceFile.GetFileName(conversionRequest.RemoveInappropriateCharactersInFileName ?? true);
        await UploadFile(projectId, fileBytes, fileName, pretranslateRequest.SourceFile.ContentType);

        await Execute(projectId, languageTaskRequest.SourceLanguage, languageTaskRequest.TargetLanguage);

        var outputFiles = await GetOutputFiles(projectId);

        // XLIFF file reference
        var xliff = outputFiles.Find(x => x.Contains("work/")) ?? throw new PluginApplicationException("No XLIFF file was created by Okapi.");
        var xliffStream = await DownloadOutputFileAsStream(projectId, xliff);
        var xliffFileReference =
            await fileManagementClient.UploadAsync(xliffStream, MimeTypes.GetMimeType(xliff), xliff.Substring(xliff.LastIndexOf('/') + 1));

        // Package file reference
        var packageStream = await DownloadOutputArchiveAsStream(projectId);
        var packageFileReference = await fileManagementClient.UploadAsync(packageStream, MimeTypes.GetMimeType(".zip"), "package.zip");

        await DeleteProject(projectId);

        return new PretranslateResponse
        {
            Xliff = xliffFileReference,
            Package = packageFileReference,
            ProjectId = projectId,
        };
    }

    internal async Task<byte[]> LoadBatchConfig(string configName)
    {
        var asm = Assembly.GetExecutingAssembly();
        Stream stream = asm.GetManifestResourceStream("Apps.Okapi.Batchconfigs." + $"{configName}.bconf");
        return await stream.GetByteData();
    }
    
    private Stream ReplaceInappropriateStrings(Stream stream)
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