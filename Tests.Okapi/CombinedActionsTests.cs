namespace Tests.Okapi;

[TestClass]
public class CombinedActionsTests : TestBase
{
    [TestMethod]
    public async Task ConvertFileToXliff_ValidFile_ReturnsXliffAndPackage()
    {
        // given
        var fileRequest = new UploadFileRequest
        {
            File = new FileReference
            {
                Name = "sample.html",
                ContentType = "text/html"
            }
        };

        var taskRequest = new ExecuteSingleLanguageTaskRequest
        {
            SourceLanguage = "en",
            TargetLanguage = "es"
        };

        var conversionRequest = new FileConversionRequest
        {
            RemoveInappropriateCharactersInFileName = false
        };

        // when
        var combinedActions = new CombinedActions(InvocationContext, FileManagementClient);
        var result = await combinedActions.ConvertFileToXliff(fileRequest, taskRequest, conversionRequest);

        // then
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task ConvertXliffToFile_InteroperableXliff_ReturnsConvertedFile()
    {
        // given
        var request = new CreateXliffResponse
        {
            Package = new FileReference { Name = Path.Join("Interoperable", "package.zip"), ContentType = "application/zip" },
            Xliff = new FileReference { Name = Path.Join("Interoperable", "__content__wknd__language-masters__en__faqs.html.xlf"), ContentType = "application/xliff+xml" }
        };

        // when
        var combinedActions = new CombinedActions(InvocationContext, FileManagementClient);
        var result = await combinedActions.ConvertXliffToFile(request);

        // then
        Assert.IsNotNull(result.File.Name.EndsWith(".html"));
    }
}
