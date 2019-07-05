using System;
using System.Collections.Generic;
using System.Text;

namespace Theatreers.Core.Abstractions
{  public interface IHashingProvider
  {
    string CalculateHash(object HashWhat);

    bool MatchHashes(object obj1, object obj2);

    bool VerifyHashMatch2Objects(object object1, object object2);

  }
}
