using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;

namespace Theatreers.Show.Models
{

  public class MessageHeaders
  {
    public string RequestCorrelationId { get; set; }
    public string RequestCreatedAt { get; set; }
    public string RequestStatus { get; set; }
  }


  public class MessageObject<T>
  {
    public MessageHeaders Headers { get; set; }
    public CosmosBaseObject<T> Body { get; set; }
  }

}