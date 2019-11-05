using Newtonsoft.Json;

namespace Theatreers.Core.Abstractions
{
  public interface IStorable
  {
    [JsonProperty("id")]
    string Id { get; set; }
  }
}
