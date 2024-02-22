using System.Xml.Serialization;

namespace Apps.Okapi.Models.Responses;

[XmlRoot("l")]
public class GetProjectsResponse
{
    [XmlElement("e")]
    public List<string> Projects { get; set; } = new();
}