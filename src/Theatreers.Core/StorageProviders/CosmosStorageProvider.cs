using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Theatreers.Core.Abstractions;
using Microsoft.Azure.Documents;
using Theatreers.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;

namespace Theatreers.Core.Providers
{
  public class CosmosStorageProvider<T> : IStorageProvider<T> where T : IPartitionable, IStorable, IValidatable, IExpirable
  {
    private IDocumentClient documentClient { get; set; }
    private Uri collectionUri { get; set; }
    private string databaseId { get; set; }
    private string collectionId { get; set; }
    public CosmosStorageProvider(IDocumentClient _documentClient, Uri _collectionUri, string _databaseId = "", string _collectionId = "")
    {
      documentClient = _documentClient;
      collectionUri = _collectionUri;
      databaseId = _databaseId;
      collectionId = _collectionId;
      
    }


    public async Task<bool> CheckExistsAsync(T _object)
    {
      IQueryable<T> query = await Query();
      if (query.Where(doc => doc.Id == _object.Id && doc.Partition == _object.Partition).Count() > 0){
        return true;
      }

      return false;
    }

    public async Task<IQueryable<T>> Query()
    {
      return documentClient.CreateDocumentQuery<T>(collectionUri).AsQueryable();
    }

    public async Task<T> ReadAsync(T _object)
    {
      return await Task.Run(() => Query().Result.Where(e => e.Id == _object.Id && e.Partition == _object.Partition).Take(1).AsEnumerable().FirstOrDefault());
    }

    public async Task CreateAsync(T _object)
    {
      if (_object.IsValid())
      {
        await documentClient.CreateDocumentAsync(collectionUri, _object);
      }
      else
      {
        throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
      }
    }

    public async Task<bool> UpdateAsync(T _object)
    {
      if (await CheckExistsAsync(_object))
      {
        if (_object.IsValid())
        {
          ResourceResponse<Document> upsert = await documentClient.UpsertDocumentAsync(collectionUri, _object);
          if (upsert.Resource != null)
          {
            return true;
          }
          return false;
        }
        else
        {
          throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
        }
      }
      throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
    }

    public async Task UpsertAsync(T _object)
    {
      if (_object.IsValid())
      {
        await documentClient.UpsertDocumentAsync(collectionUri, _object);
      }
      else
      {
        throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
      }
    }

    public async Task<bool> DeleteAsync(T _object)
    {
      if (await CheckExistsAsync(_object)) 
      {
        T _deletedObject = await ReadAsync(_object);
        _deletedObject.Ttl = 10;
      await UpsertAsync(_deletedObject);
      return true;
      }
      return false;
    }
  }
}
