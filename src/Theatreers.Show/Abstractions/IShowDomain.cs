using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Theatreers.Core.Models;
using Theatreers.Show.Models;

namespace Theatreers.Show.Abstractions
{
  public interface IShowDomain
  {
    Task<ShowObject> GetShow(string id);
    Task<ICollection<ShowListObject>> GetShowList(string letter);
    Task<ICollection<ImageObject>> GetImageByShow(string id);
    Task<ICollection<NewsObject>> GetNewsByShow(string id);
    Task CreateImageObject(MessageObject<ImageObject> message, ILogger log);
    Task CreateImageObjectsFromSearch(MessageObject<ShowObject> message, ILogger log);
    Task CreateNewsObject(MessageObject<NewsObject> message, ILogger log);
    Task CreateNewsObjectsFromSearch(MessageObject<ShowObject> message, ILogger log);
  }
}