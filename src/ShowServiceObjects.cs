using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Theatreers.Show
{

public interface IShowObject 
{
  }

  public class CosmosBaseObject<T>
  {
    public string id { get; set; }
    public T innerobject { get; set; }
    public string showId { get; set; }
    public int ttl { get; set; } = -1;
    public string isDeleted { get; set; }
    public string doctype { get; set; }
  }

  public class ImageObject : IShowObject
  {
    public string imageId { get; set; }
    public string contentUrl { get; set; }
    public string hostPageUrl { get; set; }
    public string name { get; set; }
    public string doctype { get; set; }
  }

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

  public class NewsObject : IShowObject
  {
    public string DatePublished { get; set; }
    public string BingId { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string doctype { get; set; }
  }

  public class ShowObject : IShowObject
  {

    public string id { get; set; }
    public string showName { get; set; }
    public string description { get; set; }
    public string composer { get; set; }
    public string author { get; set; }
  }

  public class ShowListObject
  {
    public string showName { get; set; }
    public string partition { get; set; }
    public int ttl { get; set; }
  }
}