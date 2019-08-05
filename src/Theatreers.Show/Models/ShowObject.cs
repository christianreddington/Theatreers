using Newtonsoft.Json;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;

namespace Theatreers.Show.Models
{
  public class ShowObject : PartitionableStorableValidatableBaseObject
  {
    public ShowObject() : base()
    {
      ValidationRules.Add(new ValidationRule() { AppliedToField = "ShowName", Mandatory = true });
    }
    [JsonProperty("showName")]
    public string ShowName { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("composer")]
    public string Composer { get; set; }
    [JsonProperty("author")]
    public string Author { get; set; }
  }
}