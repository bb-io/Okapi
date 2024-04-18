using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Okapi.Utils;

public static class FileReferenceExtensions
{
    private const string StringToReplace = "{s}";

    public static string GetFileName(this FileReference fileReference, bool removeInappropriateCharacters)
    {
        return removeInappropriateCharacters
            ? ReplaceInappropriateCharacters(fileReference.Name)
            : fileReference.Name;
    }
    
    public static string RestoreInappropriateCharacters(string fileName)
    {
        return fileName.Replace(StringToReplace, @"\");
    }
    
    private static string ReplaceInappropriateCharacters(string fileName)
    {
        return fileName.Replace(@"\", StringToReplace);
    }
}