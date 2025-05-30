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

    [TestMethod]
    public async Task UploadTranslationAssets_WithTmxAndSrx_ReturnsUploadedFileNames()
    {
        // given
        var request = new UploadTranslationAssetsRequest
        {
            LonghornWorkDir = LonghornWorkDir,
            Tmx = new FileReference
            {
                Name = "sample.tmx",
                ContentType = "application/xml"
            },
            Srx = new FileReference
            {
                Name = "sample.srx",
                ContentType = "application/xml"
            }
        };

        // when
        var combinedActions = new CombinedActions(InvocationContext, FileManagementClient);
        var result = await combinedActions.UploadTranslationAssets(request);

        // then
        Assert.IsTrue(result.TMX.StartsWith(LonghornWorkDir));
        Assert.IsTrue(result.TMX.EndsWith("/input/sample.tmx"));

        Assert.IsTrue(result.SRX?.StartsWith(LonghornWorkDir));
        Assert.IsTrue(result.SRX?.EndsWith("/input/sample.srx"));
    }

    [TestMethod]
    public async Task UploadTranslationAssets_WithTmxAndSrxOnWindowsServer_ReturnsWindowsFilepaths()
    {
        // given
        var fakeLonghornWorkDir = "C:\\Users\\TestUser\\Documents\\Okapi-Longhorn-Files";
        var request = new UploadTranslationAssetsRequest
        {
            LonghornWorkDir = fakeLonghornWorkDir,
            Tmx = new FileReference
            {
                Name = "sample.tmx",
                ContentType = "application/xml"
            },
            Srx = new FileReference
            {
                Name = "sample.srx",
                ContentType = "application/xml"
            }
        };

        // when
        var combinedActions = new CombinedActions(InvocationContext, FileManagementClient);
        var result = await combinedActions.UploadTranslationAssets(request);

        // then
        Assert.IsTrue(result.TMX.StartsWith(fakeLonghornWorkDir));
        Assert.IsTrue(result.TMX.EndsWith("\\input\\sample.tmx"));

        Assert.IsTrue(result.SRX?.StartsWith(fakeLonghornWorkDir));
        Assert.IsTrue(result.SRX?.EndsWith("\\input\\sample.srx"));
    }

    [TestMethod]
    public async Task UploadTranslationAssets_WithOnlyTmx_ReturnsUploadedFileName()
    {
        // given
        var request = new UploadTranslationAssetsRequest
        {
            LonghornWorkDir = LonghornWorkDir,
            Tmx = new FileReference
            {
                Name = "sample.tmx",
                ContentType = "application/xml"
            }
        };

        // when
        var combinedActions = new CombinedActions(InvocationContext, FileManagementClient);
        var result = await combinedActions.UploadTranslationAssets(request);

        // then
        Assert.IsTrue(result.TMX.StartsWith(LonghornWorkDir));
        Assert.IsTrue(result.TMX.EndsWith("/input/sample.tmx"));
        Assert.IsNull(result.SRX);
    }
}
