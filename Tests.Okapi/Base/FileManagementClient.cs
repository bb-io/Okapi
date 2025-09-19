using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Tests.Okapi.Base;

public class FileManagementClient : IFileManagementClient
{
    private readonly string _folderLocation;

    public FileManagementClient(string folderLocation)
    {
        _folderLocation = folderLocation ?? throw new ArgumentNullException(nameof(folderLocation));
    }

    public async Task<Stream> DownloadAsync(FileReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        var path = Path.Combine(_folderLocation, "Input", reference.Name);
        return await Task.FromResult(File.OpenRead(path)); // to keep method signature same as in SDK
    }

    public async Task<FileReference> UploadAsync(Stream stream, string contentType, string fileName)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));

        var path = Path.Combine(_folderLocation, "Output", fileName);
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using (var fileStream = File.Create(path))
        {
            await stream.CopyToAsync(fileStream);
        }

        return new FileReference { Name = fileName };
    }

    /// <summary>
    /// Utility method to grab files from Input/Output folders for test assertions.
    /// </summary>
    /// <param name="subfolder">Input or Output are expected to be used.</param>
    /// <param name="filename">Filename including extension</param>
    public async Task<Byte[]> GetFileFromTestSubfolder(string subfolder, string filename)
    {
        if (string.IsNullOrWhiteSpace(subfolder) || string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentNullException("Subfolder and Filename are expected.");
        }

        var path = Path.Combine(_folderLocation, subfolder, filename);
        return await File.ReadAllBytesAsync(path);
    }
}
