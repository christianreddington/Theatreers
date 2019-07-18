using System;
using System.Collections.Generic;
using System.Text;
using Theatreers.Core.Abstractions;

namespace Theatreers.Core.Models
{
  public class TestObject : ValidatableBaseObject, IStorableValidatable, IStorable, IValidatable
  {
    public TestObject() : base()
    {
      ValidationRules.Add(new ValidationRule() { AppliedToField = "InnerObject", Mandatory = true });
    }
    public string InnerObject { get; set; }
  }
}
