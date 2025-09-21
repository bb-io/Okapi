using System.Xml.Linq;

namespace Tests.Okapi;

/// How to check that everything works?
/// Run CreateTm_FromTMX_Works or CreateTm_FromZippedTMX_Works first to create a TM in your LONGHORN_WORKDIR.
///     Update the local TmPath constant with obtained TM path.
/// Then run UpdateTm
/// Then run DownloadTm to download the TMX export of the TM.
/// Finally, you use OKAPI locally (Rainbow or Tiakl) to convert the PensieveTM to TMX if needed.
///          example command: `tikal.bat -exp tm.pentm -sl en -tl es`
///          You should see content from both sample.tmx and contentful.xliff in the output TMX file.
[TestClass]
public class TmActionsTests : TestBase
{
    private TmActions Actions => new(InvocationContext, FileManagementClient);
    private static string TmPath => "C:\\Users\\alex-\\Okapi-Longhorn-Files\\52\\output\\tm.pentm"; // Use output from CreateTm test

    [TestMethod]
    public async Task CreateTm_FromTMX_Works()
    {
        // given
        var request = new CreateTmRequest
        {
            LonghordWorkDir = LonghornWorkDir,
            Tmx = new FileReference { Name = "sample.tmx", ContentType = "application/xml" },
            SourceLanguage = "en",
            TargetLanguages = ["es"],
        };

        // when
        var result = await Actions.CreateTm(request);

        // then
        Print(result);
        Assert.StartsWith(LonghornWorkDir, result.TmPath);
        Assert.EndsWith(".pentm", result.TmPath); // eg, LONGHORN_WORKDIR\PROJECT_ID\output\tm.pentm
    }

    [TestMethod]
    public async Task CreateTm_FromZippedTMX_Works()
    {
        // given
        var request = new CreateTmRequest
        {
            LonghordWorkDir = LonghornWorkDir,
            Tmx = new FileReference { Name = "sample-tmx.zip", ContentType = "application/xml" },
            SourceLanguage = "en",
            TargetLanguages = ["es"],
        };

        // when
        var result = await Actions.CreateTm(request);

        // then
        Print(result);
        Assert.StartsWith(LonghornWorkDir, result.TmPath);
        Assert.EndsWith(".pentm", result.TmPath); // eg, LONGHORN_WORKDIR\PROJECT_ID\output\tm.pentm
    }

    [TestMethod]
    public async Task UpdateTm_Works()
    {
        // given
        var updateRequest = new UpdateTmRequest
        {
            BilingualFile = new FileReference { Name = "contentful.xliff", ContentType = "application/xliff+xml" },
            TmPath = TmPath,
        };

        // when
        var updateResult = await Actions.UpdateTm(updateRequest);

        // then
        Print(updateResult);
        Assert.IsLessThan(updateResult.TotalSegmentsInFile, updateResult.SegmentsSent);
    }

    [TestMethod]
    public async Task DownloadTm_Works()
    {
        // given
        var request = new ExportTmxRequest
        {
            TmPath = TmPath,
        };

        // when
        var result = await Actions.DownloadTm(request);

        // then
        Print(result);
        Assert.AreEqual("tm-export.zip", result.TmxFile.Name);
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
        var result = await Actions.UploadTranslationAssets(request);

        // then
        Assert.StartsWith(LonghornWorkDir, result.TMX);
        Assert.EndsWith("/input/sample.tmx", result.TMX);

        Assert.StartsWith(result.SRX, LonghornWorkDir);
        Assert.EndsWith(result.SRX, "/input/sample.srx");
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
        var result = await Actions.UploadTranslationAssets(request);

        // then
        Assert.StartsWith(fakeLonghornWorkDir, result.TMX);
        Assert.EndsWith("\\input\\sample.tmx", result.TMX);

        Assert.StartsWith(result.SRX, fakeLonghornWorkDir);
        Assert.StartsWith(result.SRX, "\\input\\sample.srx");
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
        var result = await Actions.UploadTranslationAssets(request);

        // then
        Assert.StartsWith(LonghornWorkDir, result.TMX);
        Assert.EndsWith("/input/sample.tmx", result.TMX);
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

        // upload assets to OKAPI
        var uploadAssetsRequest = new UploadTranslationAssetsRequest
        {
            LonghornWorkDir = longhornWorkDir,
            Tmx = tmxFile,
            Srx = srxFile
        };
        var assetsResponse = await Actions.UploadTranslationAssets(uploadAssetsRequest);

        var pretranslateRequest = new PretranslateRequest
        {
            File = htmlFile,
            TmPath = assetsResponse.TMX,
            SrxPath = assetsResponse.SRX
        };

        // when
        await Actions.Pretranslate(pretranslateRequest, taskRequest, conversionRequest);

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
            TmPath = TmPath,
        };

        // when
        await Actions.Pretranslate(pretranslateRequest, taskRequest, conversionRequest);

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
