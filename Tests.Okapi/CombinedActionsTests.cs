using System.Xml.Linq;

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

    [TestMethod]
    public async Task Pretranslate_WithTMX_WithSegmentation_ProducesExpectedOutput()
    {
        // given
        var longhornWorkDir = LonghornWorkDir;
        var tmxFile = new FileReference { Name = "sample.tmx", ContentType = "application/xml" };
        var srxFile = new FileReference { Name = "sample.srx", ContentType = "application/xml" };
        var htmlFile = new FileReference { Name = "sample.html", ContentType = "text/html" };

        var taskRequest = new ExecuteSingleLanguageTaskRequest
        {
            SourceLanguage = "en",
            TargetLanguage = "es"
        };

        var conversionRequest = new FileConversionRequest { RemoveInappropriateCharactersInFileName = false };

        var combinedActions = new CombinedActions(InvocationContext, FileManagementClient);

        // upload assets to OKAPI
        var uploadAssetsRequest = new UploadTranslationAssetsRequest
        {
            LonghornWorkDir = longhornWorkDir,
            Tmx = tmxFile,
            Srx = srxFile
        };
        var assetsResponse = await combinedActions.UploadTranslationAssets(uploadAssetsRequest);

        var pretranslateRequest = new PretranslateRequest
        {
            File = htmlFile,
            TmPath = assetsResponse.TMX,
            SrxPath = assetsResponse.SRX
        };

        // when
        await combinedActions.Pretranslate(pretranslateRequest, taskRequest, conversionRequest);

        // then
        var resultingFile = XDocument.Parse(await FileManagementClient.GetFileFromTestSubfolder("Output", "sample.html.xlf"));
        var expectedFile = XDocument.Parse(await FileManagementClient.GetFileFromTestSubfolder("Input", "expected-sample.html.xlf"));

        RemoveFlackyXliffAttributes(resultingFile);
        RemoveFlackyXliffAttributes(expectedFile);

        Assert.IsTrue(
            XNode.DeepEquals(resultingFile, expectedFile),
            "Pretranslated XLIFF does not match expected output."
        );
    }

    [TestMethod]
    public async Task Pretranslate_WithExistingPensieveTM_NoSegmentation_ProducesExpectedOutput()
    {
        // given
        var taskRequest = new ExecuteSingleLanguageTaskRequest
        {
            SourceLanguage = "en",
            TargetLanguage = "es" // should match languages in the sample.tmx
        };

        var conversionRequest = new FileConversionRequest { RemoveInappropriateCharactersInFileName = false };

        var pretranslateRequest = new PretranslateRequest
        {
            File = new FileReference { Name = "sample-untranslated.html", ContentType = "text/html" },
            TmPath = "C:\\Users\\alex-\\Okapi-Longhorn-Files\\52\\output\\tm.pentm", // see TmActionsTests
        };

        // when
        var combinedActions = new CombinedActions(InvocationContext, FileManagementClient);
        await combinedActions.Pretranslate(pretranslateRequest, taskRequest, conversionRequest);

        // then
        var resultingFile = XDocument.Parse(await FileManagementClient.GetFileFromTestSubfolder("Output", "sample.html.xlf"));
        var expectedFile = XDocument.Parse(await FileManagementClient.GetFileFromTestSubfolder("Input", "expected-sample.html.xlf"));

        RemoveFlackyXliffAttributes(resultingFile);
        RemoveFlackyXliffAttributes(expectedFile);

        // then
        Assert.IsTrue(
            XNode.DeepEquals(resultingFile, expectedFile),
            "Pretranslated XLIFF does not match expected output."
        );
    }

    private static void RemoveFlackyXliffAttributes(XDocument xliff)
    {
        xliff.Descendants()
              .Where(e => e.Name.LocalName == "file")
              .Attributes("original")
              .Remove();

        xliff.Descendants()
              .Where(e => e.Name.LocalName == "match")
              .Attributes("origin")
              .Remove();
    }
}
