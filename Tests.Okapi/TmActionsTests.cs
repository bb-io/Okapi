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
        Assert.IsTrue(result.TmPath.StartsWith(LonghornWorkDir));
        Assert.IsTrue(result.TmPath.EndsWith(".pentm")); // eg, LONGHORN_WORKDIR\PROJECT_ID\output\tm.pentm
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
        Assert.IsTrue(result.TmPath.StartsWith(LonghornWorkDir));
        Assert.IsTrue(result.TmPath.EndsWith(".pentm")); // eg, LONGHORN_WORKDIR\PROJECT_ID\output\tm.pentm
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
        Assert.IsTrue(updateResult.SegmentsSent < updateResult.TotalSegmentsInFile);
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
}
