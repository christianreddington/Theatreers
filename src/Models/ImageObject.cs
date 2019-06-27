using Newtonsoft.Json;
using Theatreers.Show.Abstractions;

namespace Theatreers.Show.Models
{
  public class ImageObject : IShowObject
  {
    [JsonProperty("ImageId")]
    public string ImageId { get; set; }
    [JsonProperty("contentUrl")]
    public string ContentUrl { get; set; }
    [JsonProperty("hostPageUrl")]
    public string HostPageUrl { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("doctype")]
    public string Doctype { get; set; }
  }
}