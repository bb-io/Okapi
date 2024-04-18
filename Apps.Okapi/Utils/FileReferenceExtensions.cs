using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Utils;

public static class FileReferenceExtensions
{
    public static string GetFileName(this FileReference fileReference)
    {
        return RemoveInappropriateCharacters(fileReference.Name);
    }
    
    private static string RemoveInappropriateCharacters(string fileName)
    {
        if (fileName.StartsWith(@"\"))
        {
            fileName = fileName.Remove(0, 1);
        }
        
        return fileName.Replace(@"\", "-");
    }
}