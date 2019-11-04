using System;
using System.Collections.Generic;
using System.Text;

namespace Theatreers.Core.Abstractions
{
  public interface IPartitionableStorableValidatable : IPartitionable, IStorableValidatable, IExpirable
  {
  }
}
