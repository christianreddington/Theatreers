using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Theatreers.Core.Abstractions;

namespace Theatreers.Core.Models
{
  public abstract class PartitionableStorableValidatableBaseObject : ValidatableBaseObject, IPartitionableStorableValidatable
  {
    public PartitionableStorableValidatableBaseObject() : base()
    {
      ValidationRules.Add(new ValidationRule() { AppliedToField = "Partition", Mandatory = true });
      ValidationRules.Add(new ValidationRule() { AppliedToField = "Doctype", Mandatory = true });
    }
    //[JsonProperty("innerObject")]
    // public T InnerObject { get; set; }
    [JsonProperty("doctype")]
    public string Doctype { get; set; }
    [JsonProperty("isDeleted")]
    public bool IsDeleted { get; set; } = false;
    [JsonProperty("partition")]
    public string Partition { get; set; }
    [JsonProperty("ttl")]
    public int Ttl { get; set; } = -1;
  }
}
