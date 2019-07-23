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
      CosmosBaseObject<ImageObject> _object = _message.Body;
      IStorageProvider<CosmosBaseObject<ImageObject>> store = new CosmosStorageProvider<CosmosBaseObject<ImageObject>>(client, _collectionUri);
      await store.CreateAsync(_object);
    }

    public async Task CreateNewsAsync(IDocumentClient client, MessageObject<NewsObject> _message)
    {
      CosmosBaseObject<NewsObject> _object = _message.Body;
      IStorageProvider<CosmosBaseObject<NewsObject>> store = new CosmosStorageProvider<CosmosBaseObject<NewsObject>>(client, _collectionUri);
      await store.CreateAsync(_object);
    }


    public async Task CreateShowAsync(IDocumentClient client, MessageObject<ShowObject> _message)
    {
      CosmosBaseObject<ShowObject> _object = _message.Body;
      IStorageProvider<CosmosBaseObject<ShowObject>> store = new CosmosStorageProvider<CosmosBaseObject<ShowObject>>(client, _collectionUri);
      await store.CreateAsync(_object);
    }

    public async Task DeleteImageAsync(IDocumentClient client, CosmosBaseObject<ImageObject> _object)
    {
      IStorageProvider<CosmosBaseObject<ImageObject>> store = new CosmosStorageProvider<CosmosBaseObject<ImageObject>>(client, _collectionUri);
      await store.DeleteAsync(_object);
    }

    public async Task DeleteNewsAsync(IDocumentClient client, CosmosBaseObject<NewsObject> _object)
    {
      IStorageProvider<CosmosBaseObject<NewsObject>> store = new CosmosStorageProvider<CosmosBaseObject<NewsObject>>(client, _collectionUri);
      await store.DeleteAsync(_object);
    }
    public async Task DeleteShowAsync(IDocumentClient client, CosmosBaseObject<ShowObject> _object)
    {
      IStorageProvider<CosmosBaseObject<ShowObject>> store = new CosmosStorageProvider<CosmosBaseObject<ShowObject>>(client, _collectionUri);
      await store.DeleteAsync(_object);
    }

    public async Task<ICollection<CosmosBaseObject<ImageObject>>> GetImagesByShowAsync(IDocumentClient client, string showId)
    {
      IStorageProvider<CosmosBaseObject<ImageObject>> store = new CosmosStorageProvider<CosmosBaseObject<ImageObject>>(client, _collectionUri);
      IQueryable<CosmosBaseObject<ImageObject>> query = await store.Query();
      return query.Where(e => e.Partition == showId && e.Doctype == DocTypes.Image).ToList();
    }

    public async Task<ICollection<CosmosBaseObject<NewsObject>>> GetNewsByShowAsync(IDocumentClient client, string showId)
    {
      IStorageProvider<CosmosBaseObject<NewsObject>> store = new CosmosStorageProvider<CosmosBaseObject<NewsObject>>(client, _collectionUri);
      IQueryable<CosmosBaseObject<NewsObject>> query = await store.Query();
      return query.Where(e => e.Partition == showId && e.Doctype == DocTypes.News).ToList();
    }
    public async Task<CosmosBaseObject<ShowObject>> GetShowAsync(IDocumentClient client, string showId)
    {
      IStorageProvider<CosmosBaseObject<ShowObject>> store = new CosmosStorageProvider<CosmosBaseObject<ShowObject>>(client, _collectionUri);
      CosmosBaseObject<ShowObject> _object = new CosmosBaseObject<ShowObject>()
      {
        Id = showId,
        Partition = showId
      };

      return await store.ReadAsync(_object);
    }

    public async Task<ICollection<CosmosBaseObject<ShowListObject>>> GetShowsAsync(IDocumentClient client, string partitionKey)
    {
      IStorageProvider<CosmosBaseObject<ShowListObject>> store = new CosmosStorageProvider<CosmosBaseObject<ShowListObject>>(client, _collectionUri);
      IQueryable<CosmosBaseObject<ShowListObject>> query = await store.Query();
      return query.Where(e => e.Partition == partitionKey).ToList();
    }

    public async Task<bool> UpdateShowAsync(IDocumentClient client, CosmosBaseObject<ShowObject> _object)
    {
      IStorageProvider<CosmosBaseObject<ShowObject>> store = new CosmosStorageProvider<CosmosBaseObject<ShowObject>>(client, _collectionUri);
      return await store.UpdateAsync(_object);
    }

    public async Task<bool> UpdateImageAsync(IDocumentClient client, CosmosBaseObject<ImageObject> _object)
    {
      IStorageProvider<CosmosBaseObject<ImageObject>> store = new CosmosStorageProvider<CosmosBaseObject<ImageObject>>(client, _collectionUri);
      return await store.UpdateAsync(_object);
    }

    public async Task<bool> UpdateNewsAsync(IDocumentClient client, CosmosBaseObject<NewsObject> _object)
    {
      IStorageProvider<CosmosBaseObject<NewsObject>> store = new CosmosStorageProvider<CosmosBaseObject<NewsObject>>(client, _collectionUri);
      return await store.UpdateAsync(_object);
    }



  }
}
