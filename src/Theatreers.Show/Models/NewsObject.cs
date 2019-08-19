using Newtonsoft.Json;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;

namespace Theatreers.Show.Models
{

  public class NewsObject : ShowDomainObject
  {
    public NewsObject() : base()
    {
      ValidationRules.Add(new ValidationRule() { AppliedToField = "Url", Mandatory = true });
      ValidationRules.Add(new ValidationRule() { AppliedToField = "Name", Mandatory = true });
      ValidationRules.Add(new ValidationRule() { AppliedToField = "DatePublished", Mandatory = true });
    }
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