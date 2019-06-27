using Newtonsoft.Json;
using System;
using Theatreers.Show.Abstractions;

namespace Theatreers.Show.Models
{

  public class CosmosBaseObject<T>
  {
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("innerObject")]
    public T InnerObject { get; set; }
    [JsonProperty("showId")]
    public string ShowId { get; set; }
    [JsonProperty("ttl")]
    public int Ttl { get; set; } = -1;
    [JsonProperty("isDeleted")]
    public bool IsDeleted { get; set; } = false;
    [JsonProperty("doctype")]
    public string Doctype { get; set; }
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }
}