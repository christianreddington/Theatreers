using Newtonsoft.Json;
using Theatreers.Show.Abstractions;

namespace Theatreers.Show.Models
{

  public class NewsObject : PartitionableValidatableBaseObject, IPartitionable, IValidatable
  {
    [JsonProperty("datePublished")]
    public string DatePublished { get; set; }
    [JsonProperty("bingId")]
    public string BingId { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
  }

}