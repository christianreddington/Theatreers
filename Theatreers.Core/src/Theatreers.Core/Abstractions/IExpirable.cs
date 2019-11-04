using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Theatreers.Core.Abstractions
{
  public interface IExpirable
  {
    [JsonProperty("ttl")]
    int Ttl { get; set; }
  }
}
