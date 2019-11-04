using System;
using System.Collections.Generic;
using System.Text;
using Theatreers.Core.Models;

namespace Theatreers.Core.Abstractions
{  public interface IValidatable
  {

    IDictionary<string, string> GetValidationErrors();

    bool IsValid();

    IList<ValidationRule> ValidationRules { get; set; }

  }
}
