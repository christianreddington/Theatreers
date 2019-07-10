using System;
using System.Collections.Generic;
using System.Text;

namespace Theatreers.Core.Models
{
  public class ValidationRule
  {
    public string AppliedToField;
    public string Regex;
    public bool Mandatory;
    public bool NonNull;
    public int MaxVal;
    public int MinVal;
    public int MaxLength;
    public int MinLength;

    public IList<string> AllowedValues;

  }
}
