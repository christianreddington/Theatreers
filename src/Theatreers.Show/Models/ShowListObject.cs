using Newtonsoft.Json;
using Theatreers.Core.Models;

namespace Theatreers.Show.Models
{

  public class ShowListObject : PartitionableStorableValidatableBaseObject
  {
    [JsonProperty("showName")]
    public string ShowName { get; set; }
  }
}