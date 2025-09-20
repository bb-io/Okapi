using Apps.Okapi.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Exceptions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Apps.Okapi.Utils;

public static class BatchConfig
{
    /// <summary>
    /// Generates a batch configuration overwrite XML string.
    /// This is used to customize Okapi pipeline steps, particularly for pre-translation.
    /// </summary>
    /// <param name="tmPath">The absolute path to the TMX file or PensieveTM folder, that's must be local to the OKAPI Longhorn server.</param>
    /// <param name="srxFilePath">The optional absolute path to the SRX file for custom segmentation rules (must be local to the OKAPI Longhorn server). If null, the OKAPI's default segmentation rules are used.</param>
    /// <returns>An XML string representing the batch configuration overwrite.</returns>
    /// <remarks>
    /// <para>The generated XML format is specific to the Okapi Longhorn <see href="https://gitlab.com/okapiframework/longhorn/-/blob/main/longhorn-app/src/main/java/net/sf/okapi/applications/longhorn/RESTInterface.java?ref_type=heads#L144">`RESTInterface.addBatchConfigurationFile` implementation</see>.</para>
    /// <example>
    /// An overwrite example:
    /// <code>
    /// <![CDATA[
    /// <l>
    ///     <e>
    ///         <stepClassName>net.sf.okapi.someStepName</stepClassName>
    ///         <stepParams>someParam=value</stepParams>
    ///     </e>
    /// </l>
    /// ]]>
    /// </code>
    /// </example>
    /// </remarks>
    public static string OverridePretranslateConfig(string tmPath, string? srxFilePath)
    {
        List<string> tmStepParams = Path.GetExtension(tmPath) switch
        {
            ".tmx" => [$"resourceParameters.biFile={tmPath}"],
            ".pentm" =>
            [
                "resourceClassName=net.sf.okapi.connectors.pensieve.PensieveTMConnector",
                $"resourceParameters.dbDirectory={tmPath}",
                "resourceParameters.host=http://localhost:8080/",
                "resourceParameters.useServer.b=false",
            ],
            _ => throw new PluginMisconfigurationException("TM must point to a .tmx file or a .pentm folder"),
        };

        XElement overwrite = new XElement("l",
            new XElement("e",
                new XElement("stepClassName", "net.sf.okapi.steps.leveraging.LeveragingStep"),
                new XElement("stepParams", string.Join('\n', tmStepParams))
            )
        );

        if (srxFilePath != null)
        {
            overwrite.Add(new XElement("e",
                new XElement("stepClassName", "net.sf.okapi.steps.segmentation.SegmentationStep"),
                new XElement("stepParams", $"sourceSrxPath={srxFilePath}")
            ));
        }

        return overwrite.ToString();
    }

    public static string OverrideTmImportConfig(string tmPath, bool overwriteSameSource = true)
    {
        var stepParams = new List<string>
        {
            $"tmDirectory={tmPath}",
            $"overwriteSameSource.b={(overwriteSameSource ? "true" : "false")}",
        };

        var overwrite = new XElement("l",
            new XElement("e",
                new XElement("stepClassName", "net.sf.okapi.steps.tmimport.TMImportStep"),
                new XElement("stepParams", string.Join('\n', stepParams))
            )
        );
        return overwrite.ToString();
    }
}
