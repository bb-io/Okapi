namespace Tests.Okapi;

[TestClass]
public class TmActionsTests : TestBase
{
    private TmActions Actions => new(InvocationContext, FileManagementClient);
    private static string TmPath => "C:\\Users\\alex-\\Okapi-Longhorn-Files\\39\\output\\tm.pentm"; // Use output from CreateTm test

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
        Assert.IsTrue(result.TmPath.EndsWith(".pentm")); // eg, LONGHORN_WORKDIR\PROJECT_ID\input\tm.pentm
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
        Assert.IsTrue(result.TmPath.EndsWith(".pentm")); // eg, LONGHORN_WORKDIR\PROJECT_ID\input\tm.pentm
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
    public async Task UpdateTm_Works()
    {
        // given
        var updateRequest = new UpdateTmRequest
        {
            BilingualFile = new FileReference { Name = "contentful.xliff", ContentType = "application/xliff+xml" },
            TMPath = TmPath,
        };

        // when
        var updateResult = await Actions.UpdateTm(updateRequest);

        // then
        Print(updateResult);
        Assert.IsTrue(updateResult.SegmentsSent < updateResult.TotalSegmentsInFile);
    }
}
