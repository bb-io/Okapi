using Apps.Okapi.Actions;
using Apps.Okapi.Models.Requests;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Okapi.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
}
