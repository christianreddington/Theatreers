using System;
using Theatreers.Core.Abstractions;

namespace Theatreers.Core.Models
{
  public class MessageHeaders
  {
    public string RequestCorrelationId { get; set; } = Guid.NewGuid().ToString();
    public DateTime RequestCreatedAt { get; set; } = DateTime.Now;
  }


  public class MessageObject
  {
    MessageHeaders Headers { get; set; }
    IStorableValidatable Body { get; set; }
  }
}
