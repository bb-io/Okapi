using System.Xml.Serialization;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Okapi.Models.Responses;

[XmlRoot("l")]
public class GetInputFilesResponse
{
    [XmlElement("e"), Display("File names")]
    public List<string> FileNames { get; set; } = new();
}