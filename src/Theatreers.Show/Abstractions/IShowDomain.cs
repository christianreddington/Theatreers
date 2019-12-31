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
    Task<ImageObject> GetImage(string id, string imageId);
    Task<ICollection<ShowListObject>> GetShowList(string letter);
    Task<ICollection<ImageObject>> GetImageByShow(string id);
    Task<ICollection<NewsObject>> GetNewsByShow(string id);
    Task CreateImageObject(MessageObject<ImageObject> message, ILogger log);
    Task CreateImageObjectsFromSearch(MessageObject<ShowObject> message, ILogger log, int count);
    Task CreateNewsObject(MessageObject<NewsObject> message, ILogger log);
    Task CreateNewsObjectsFromSearch(MessageObject<ShowObject> message, ILogger log, int count);
    Task CreateShowObject(MessageObject<ShowObject> message);
    Task<bool> DeleteImageObject(MessageObject<ImageObject> message);
    Task<bool> DeleteNewsObject(MessageObject<NewsObject> message);
    Task<bool> DeleteShowObject(MessageObject<ShowObject> message);
    Task<bool> UpdateShowObject(MessageObject<ShowObject> message);
    Task<bool> UpdateImageObject(MessageObject<ImageObject> message);
    Task<bool> UpdateNewsObject(MessageObject<NewsObject> message);
    Task<bool> SaveToBlobStorage(string blobName, string showGuid, byte[] data);
  }
}