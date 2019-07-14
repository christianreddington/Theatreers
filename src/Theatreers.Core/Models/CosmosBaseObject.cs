using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Theatreers.Core.Abstractions;

namespace Theatreers.Core.Models
{
  public class CosmosBaseObject<T> : ValidatableBaseObject, IPartitionable
  {
    public CosmosBaseObject() : base()
    {
      ValidationRules.Add(new ValidationRule() { AppliedToField = "Id", Mandatory = true });
      ValidationRules.Add(new ValidationRule() { AppliedToField = "InnerObject", Mandatory = true });
      ValidationRules.Add(new ValidationRule() { AppliedToField = "Partition", Mandatory = true });
      ValidationRules.Add(new ValidationRule() { AppliedToField = "Doctype", Mandatory = true });
    }
    [JsonProperty("innerObject")]
    public T InnerObject { get; set; }
    [JsonProperty("partition")]
    public string Partition { get; set; }
    [JsonProperty("isDeleted")]
    public bool IsDeleted { get; set; } = false;
    [JsonProperty("doctype")]
    public string Doctype { get; set; }
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }
}
