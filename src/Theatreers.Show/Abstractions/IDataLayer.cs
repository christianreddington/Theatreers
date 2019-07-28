using System.Collections.Generic;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Models;

namespace Theatreers.Show.Abstractions
{
  public interface IDataLayer
  {
    Task CreateImageAsync(MessageObject<ImageObject> _message);
    Task CreateNewsAsync(MessageObject<NewsObject> _message);
    Task CreateShowAsync(MessageObject<ShowObject> _message);
    Task DeleteImageAsync(ImageObject _object);
    Task DeleteNewsAsync(NewsObject _object);
    Task DeleteShowAsync(ShowObject _object);
    Task<ICollection<ImageObject>> GetImagesByShowAsync(string showId);
    Task<ICollection<NewsObject>> GetNewsByShowAsync(string showId);
    Task<ShowObject> GetShowAsync(string showId);
    Task<ICollection<ShowListObject>> GetShowsAsync(string partitionKey);
    Task<bool> UpdateImageAsync(ImageObject _object);
    Task<bool> UpdateNewsAsync(NewsObject _object);
    Task<bool> UpdateShowAsync(ShowObject _object);
  }
}