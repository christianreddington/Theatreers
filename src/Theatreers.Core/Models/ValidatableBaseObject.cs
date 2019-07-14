using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Theatreers.Core.Abstractions;

namespace Theatreers.Core.Models
{
  public class ValidatableBaseObject : IValidatable, IStorable, IReplicable
  {
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("ttl")]
    public int Ttl { get; set; } = -1;

    public int Version { get; set; }

    [JsonIgnore]
    public IList<ValidationRule> ValidationRules { get; set; } = new List<ValidationRule>();

    public virtual IDictionary<string, string> GetValidationErrors()
    {
      IDictionary<string, string> resultfails = new Dictionary<string, string>();
      foreach (ValidationRule rule in ValidationRules)
      {
        // Pick up and cast the aspect we need to test.
        dynamic thiscasted = this;
      

        string castme = Convert.ToString(thiscasted.GetType().GetProperty(rule.AppliedToField).GetValue(thiscasted));

        if (castme == null){
          castme = "";
        }

      //.GetValue(thiscasted).ToString();

        //string castme = thiscasted[rule.AppliedToField] ;

        // Regex Testing 
        if (rule.Regex != null)
        {

          Regex rex = new Regex(rule.Regex);
          if (!rex.IsMatch(castme)) resultfails.Add(rule.AppliedToField, $"Failed:RegexMatch:{rule.Regex}");

        }

        // Mandatory (not null or "")
        if (rule.Mandatory && castme == "") resultfails.Add(rule.AppliedToField, $"Failed:Mandatory");
        if (rule.MaxVal != 0 && int.Parse(castme) > rule.MaxVal) resultfails.Add(rule.AppliedToField, $"Failed:Value Exceeds allowed maximum {rule.MaxVal}");
        if (rule.MinVal != 0 && int.Parse(castme) > rule.MinVal) resultfails.Add(rule.AppliedToField, $"Failed:Value too small allowed min {rule.MinVal}");
        if (rule.MaxLength != 0 && castme.Length > rule.MaxLength) resultfails.Add(rule.AppliedToField, $"Failed:Length Exceeds allowed maximum {rule.MaxLength}");
        if (rule.MinLength != 0 && castme.Length > rule.MinLength) resultfails.Add(rule.AppliedToField, $"Failed:Length too small, allowed minimum {rule.MinLength}");

        if (rule.AllowedValues != null && rule.AllowedValues.Contains(castme))
          resultfails.Add(
              rule.AppliedToField,
              $"Failed:Not in list of allowed values ({string.Join(",", rule.AllowedValues.ToArray())})"
              );

      }

      return resultfails;
    }

    public virtual bool IsValid()
    {
      return (GetValidationErrors().Count == 0);
    }
  }
}
