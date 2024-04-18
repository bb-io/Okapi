using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Utils;

public static class FileReferenceExtensions
{
    public static string GetFileName(this FileReference fileReference)
    {
        return RemoveInappropriateCharacters(fileReference.Name);
    }
    
    public static string RestoreInappropriateCharacters(string fileName)
    {
        return fileName.Replace("|", @"\");
    }
    
    private static string RemoveInappropriateCharacters(string fileName)
    {
        return fileName.Replace(@"\", "|");
    }
}