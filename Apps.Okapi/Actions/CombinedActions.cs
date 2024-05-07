using Apps.Okapi.Invocables;
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

namespace Apps.Okapi.Actions
{
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

            if (fileRequest.File.Name.EndsWith(".pdf"))
            {
                fileRequest.File.Name = fileRequest.File.Name.Replace(".pdf", ".doc"); // Okapi requires .docx files
            }

            await UploadFile(projectId, fileBytes, fileRequest.File.GetFileName(conversionRequest.RemoveInappropriateCharactersInFileName ?? true), fileRequest.File.ContentType);

            await Execute(projectId, request.SourceLanguage, request.TargetLanguage);

            var outputFiles = await GetOutputFiles(projectId);

            var xliff = outputFiles.Find(x => x.Contains("work/"));

            if (xliff == null) throw new Exception("No XLIFF file was created by Okapi.");

            // XLIFF file reference
            var xliffStream = await DownloadOutputFileAsStream(projectId, xliff);
            var xliffFileReference =
                await fileManagementClient.UploadAsync(xliffStream, MimeTypes.GetMimeType(xliff), xliff.Remove(0, 10));

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
                    if (xliffEntry == null) throw new Exception("The package did not contain any XLIFF files");
                    xliffEntry.Delete();
                    xliffEntry = archive.CreateEntry($"work/{xliffEntry.Name}");

                    using var entryStream = xliffEntry.Open();
                    await xliffStream.CopyToAsync(entryStream);
                }

                var fileBytes = memoryStream.ToArray();

                await UploadFile(projectId, fileBytes, input.Package.Name, input.Package.ContentType);

                await Execute(projectId);

                var outputFiles = await GetOutputFiles(projectId);
                if (outputFiles.Count == 0) throw new Exception("No files were converted");
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
}