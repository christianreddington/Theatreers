using Newtonsoft.Json;

namespace Theatreers.Show.Models
{

  public class ShowListObject : PartitionableValidatableBaseObject, IPartitionable, IValidatable
  {
    [JsonProperty("showName")]
    public string ShowName { get; set; }
    [JsonProperty("partition")]
    public string Partition { get; set; }
  }
}