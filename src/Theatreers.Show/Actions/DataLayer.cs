using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theatreers.Core.Abstractions;
using Theatreers.Core.Models;
using Theatreers.Core.Providers;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;
using ImageObject = Theatreers.Show.Models.ImageObject;

namespace Theatreers.Show
{
  public class DataLayer : IDataLayer
  {
    private readonly IStorageProvider<ImageObject> _imageStore;
    private readonly IStorageProvider<NewsObject> _newsStore;
    private readonly IStorageProvider<ShowObject> _showStore;
    private readonly IStorageProvider<ShowListObject> _showListStore;

    public DataLayer(IStorageProvider<ImageObject> imageStore, IStorageProvider<NewsObject> newsStore, IStorageProvider<ShowObject> showStore, IStorageProvider<ShowListObject> showListStore)
    {
      _imageStore = imageStore;
      _newsStore = newsStore;
      _showStore = showStore;
      _showListStore = showListStore;
      // _collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
    }

    public async Task CreateImageAsync(MessageObject<ImageObject> _message)
    {
      ImageObject _object = _message.Body;
      await _imageStore.CreateAsync(_object);
    }

    public async Task CreateNewsAsync(MessageObject<NewsObject> _message)
    {
      NewsObject _object = _message.Body;
      await _newsStore.CreateAsync(_object);
    }

    public async Task CreateShowAsync(MessageObject<ShowObject> _message)
    {
      ShowObject _object = _message.Body;
      await _showStore.CreateAsync(_object);
    }

    public async Task<bool> DeleteImageAsync(ImageObject _object)
    {
      return await _imageStore.DeleteAsync(_object);
    }

    public async Task<bool> DeleteNewsAsync(NewsObject _object)
    {
      return await _newsStore.DeleteAsync(_object);
    }
    public async Task<bool> DeleteShowAsync(ShowObject _object)
    {
      return await _showStore.DeleteAsync(_object);
    }

    public async Task<ICollection<ImageObject>> GetImagesByShowAsync(string showId)
    {
      IQueryable<ImageObject> query = await _imageStore.Query();
      return query.Where(e => e.Partition == showId && e.Doctype == DocTypes.Image).ToList();
    }

    public async Task<ICollection<NewsObject>> GetNewsByShowAsync(string showId)
    {
      IQueryable<NewsObject> query = await _newsStore.Query();
      return query.Where(e => e.Partition == showId && e.Doctype == DocTypes.News).ToList();
    }
    public async Task<ShowObject> GetShowAsync(string showId)
    {
      ShowObject _object = new ShowObject()
      {
        Id = showId,
        Partition = showId
      };

      return await _showStore.ReadAsync(_object);
    }

    public async Task<ImageObject> GetImageAsync (string showId, string imageId)
    {
        ImageObject _object = new ImageObject ()
        {
            Id = imageId,
            Partition = showId
        };

        return await _imageStore.ReadAsync(_object);
    }

        public async Task<ICollection<ShowListObject>> GetShowsAsync(string partitionKey)
    {
      IQueryable<ShowListObject> query = await _showListStore.Query();
      return query.Where(e => e.Partition == partitionKey).ToList();
    }

    public async Task<bool> UpdateShowAsync(ShowObject _object)
    {
      return await _showStore.UpdateAsync(_object);
    }

    public async Task<bool> UpdateImageAsync(ImageObject _object)
    {
      return await _imageStore.UpdateAsync(_object);
    }

    public async Task<bool> UpdateNewsAsync(NewsObject _object)
    {
      return await _newsStore.UpdateAsync(_object);
    }



  }
}
