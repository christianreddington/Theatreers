using System;
using System.Collections.Generic;
using System.Text;
using Theatreers.Core.Abstractions;

namespace Theatreers.Core.Models
{
  public class PartitionedTestObject : PartitionableStorableValidatableBaseObject, IPartitionableStorableValidatable, IPartitionable, IStorable, IValidatable, IExpirable
  {
    public PartitionedTestObject() : base()
    {
      ValidationRules.Add(new ValidationRule() { AppliedToField = "InnerObject", Mandatory = true });
    }
    public string InnerObject { get; set; }
  }
}
