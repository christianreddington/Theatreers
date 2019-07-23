using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theatreers.Core;
using Theatreers.Core.Abstractions;
using Theatreers.Core.Models;
using Theatreers.Core.Providers;
using Theatreers.Show.Models;

namespace Theatreers.Show.Actions
{
  public class Actions
  {
    private Uri _collectionUri;
    public Actions(string databaseId, string collectionId)
    {
      _collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
    }
    public async Task CreateImageAsync(IDocumentClient client, MessageObject<ImageObject> _message)
    {
      ImageObject _object = _message.Body;
      IStorageProvider<ImageObject> store = new CosmosStorageProvider<ImageObject>(client, _collectionUri);
      await store.CreateAsync(_object);
    }

    public async Task CreateNewsAsync(IDocumentClient client, MessageObject<NewsObject> _message)
    {
      NewsObject _object = _message.Body;
      IStorageProvider<NewsObject> store = new CosmosStorageProvider<NewsObject>(client, _collectionUri);
      await store.CreateAsync(_object);
    }


    public async Task CreateShowAsync(IDocumentClient client, MessageObject<ShowObject> _message)
    {
      ShowObject _object = _message.Body;
      IStorageProvider<ShowObject> store = new CosmosStorageProvider<ShowObject>(client, _collectionUri);
      await store.CreateAsync(_object);
    }

    public async Task DeleteImageAsync(IDocumentClient client, ImageObject _object)
    {
      IStorageProvider<ImageObject> store = new CosmosStorageProvider<ImageObject>(client, _collectionUri);
      await store.DeleteAsync(_object);
    }

    public async Task DeleteNewsAsync(IDocumentClient client, NewsObject _object)
    {
      IStorageProvider<NewsObject> store = new CosmosStorageProvider<NewsObject>(client, _collectionUri);
      await store.DeleteAsync(_object);
    }
    public async Task DeleteShowAsync(IDocumentClient client, ShowObject _object)
    {
      IStorageProvider<ShowObject> store = new CosmosStorageProvider<ShowObject>(client, _collectionUri);
      await store.DeleteAsync(_object);
    }

    public async Task<ICollection<ImageObject>> GetImagesByShowAsync(IDocumentClient client, string showId)
    {
      IStorageProvider<ImageObject> store = new CosmosStorageProvider<ImageObject>(client, _collectionUri);
      IQueryable<ImageObject> query = await store.Query();
      return query.Where(e => e.Partition == showId && e.Doctype == DocTypes.Image).ToList();
    }

    public async Task<ICollection<NewsObject>> GetNewsByShowAsync(IDocumentClient client, string showId)
    {
      IStorageProvider<NewsObject> store = new CosmosStorageProvider<NewsObject>(client, _collectionUri);
      IQueryable<NewsObject> query = await store.Query();
      return query.Where(e => e.Partition == showId && e.Doctype == DocTypes.News).ToList();
    }
    public async Task<ShowObject> GetShowAsync(IDocumentClient client, string showId)
    {
      IStorageProvider<ShowObject> store = new CosmosStorageProvider<ShowObject>(client, _collectionUri);
      ShowObject _object = new ShowObject()
      {
        Id = showId,
        Partition = showId
      };

      return await store.ReadAsync(_object);
    }

    public async Task<ICollection<ShowListObject>> GetShowsAsync(IDocumentClient client, string partitionKey)
    {
      IStorageProvider<ShowListObject> store = new CosmosStorageProvider<ShowListObject>(client, _collectionUri);
      IQueryable<ShowListObject> query = await store.Query();
      return query.Where(e => e.Partition == partitionKey).ToList();
    }

    public async Task<bool> UpdateShowAsync(IDocumentClient client, ShowObject _object)
    {
      IStorageProvider<ShowObject> store = new CosmosStorageProvider<ShowObject>(client, _collectionUri);
      return await store.UpdateAsync(_object);
    }

    public async Task<bool> UpdateImageAsync(IDocumentClient client, ImageObject _object)
    {
      IStorageProvider<ImageObject> store = new CosmosStorageProvider<ImageObject>(client, _collectionUri);
      return await store.UpdateAsync(_object);
    }

    public async Task<bool> UpdateNewsAsync(IDocumentClient client, NewsObject _object)
    {
      IStorageProvider<NewsObject> store = new CosmosStorageProvider<NewsObject>(client, _collectionUri);
      return await store.UpdateAsync(_object);
    }



  }
}
