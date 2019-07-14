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
  public class CosmosStorageProvider<T> : IStorageProvider<T> where T : ValidatableBaseObject, IPartitionable
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


    public async Task<bool> CheckExistsAsync(string reference, string partition, ILogger log)
    {
      IQueryable<T> query = await Query();
      if (query.Where(doc => doc.Id == reference && doc.Partition == partition).Count() > 0){
        return true;
      }

      return false;
    }

    public async Task<IQueryable<T>> Query()
    {
      return documentClient.CreateDocumentQuery<T>(collectionUri);
    }

    public async Task<T> ReadAsync(string reference, string partition)
    {
      return await Task.Run(() => Query().Result.Where(e => e.Partition == partition).Take(1).AsEnumerable().FirstOrDefault());
    }

    public async Task CreateAsync(T _object, ILogger log)
    {

      if (_object.IsValid())
      {
        try
        {
          await documentClient.CreateDocumentAsync(collectionUri, _object);
        }
        catch (Exception ex)
        {
          log.LogError($"Create Object Failed: {ex.Message}");

        }
      }
      else
      {
        throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
      }
    }

    public async Task<bool> UpdateAsync(string reference, T _object, ILogger log)
    {
      _object.Id = reference;
      if (_object.IsValid())
      {
        try
        {
          ResourceResponse<Document> upsert = await documentClient.UpsertDocumentAsync(collectionUri, _object);
          if (upsert.Resource != null)
          {
            return true;
          }

          return false;
        }
        catch (Exception ex)
        {
          log.LogError($"Create Object Failed: {ex.Message}");
          return false;
        }
      }
      else
      {
        throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
      }
    }

    public async Task UpsertAsync(string reference, T _object, ILogger log)
    {
      _object.Id = reference;
      if (_object.IsValid())
      {
        try
        {
          await documentClient.UpsertDocumentAsync(collectionUri, _object);
        }
        catch (Exception ex)
        {
          log.LogError($"Create Object Failed: {ex.Message}");
        }
      }
      else
      {
        throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
      }
    }

    public async Task<bool> DeleteAsync(string reference, string partition, ILogger log)
    {
      IQueryable<T> query = await Query();
      T _object = query.Where(e => e.Id == reference && e.Partition == partition).AsEnumerable().FirstOrDefault();
      _object.Ttl = 1;
      if (_object.IsValid())
      {
          await documentClient.UpsertDocumentAsync(collectionUri, _object);
          return true;
      }
      else
      {
        throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
      } 
    }
  }
}
