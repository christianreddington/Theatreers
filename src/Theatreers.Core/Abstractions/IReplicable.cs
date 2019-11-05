using System;
using System.Collections.Generic;
using System.Text;

namespace Theatreers.Core.Abstractions
{
  public interface IReplicable
  {
    int Version { get; set; }
  }
}
