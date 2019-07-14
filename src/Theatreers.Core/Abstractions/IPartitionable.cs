using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Theatreers.Core.Abstractions
{
  public interface IPartitionable
  {

    [JsonProperty("partition")]
    string Partition { get; set; }
  }
}
