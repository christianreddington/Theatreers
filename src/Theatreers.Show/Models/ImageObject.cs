using Newtonsoft.Json;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;

namespace Theatreers.Show.Models
{
  public class ImageObject : PartitionableStorableValidatableBaseObject
  {
    public ImageObject() : base()
    {
      ValidationRules.Add(new ValidationRule() { AppliedToField = "ContentUrl", Mandatory = true });
      ValidationRules.Add(new ValidationRule() { AppliedToField = "Name", Mandatory = true });
    }

    [JsonProperty("ImageId")]
    public string ImageId { get; set; }
    [JsonProperty("contentUrl")]
    public string ContentUrl { get; set; }
    [JsonProperty("hostPageUrl")]
    public string HostPageUrl { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
  }
}