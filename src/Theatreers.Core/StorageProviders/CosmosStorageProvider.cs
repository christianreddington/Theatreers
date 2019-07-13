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
  public class CosmosStorageProvider<T> : IStorageProvider<T> where T : ValidatableBaseObject
  {
    private IDocumentClient documentClient { get; set; }
    private Uri collectionUri { get; set; }
    public CosmosStorageProvider(IDocumentClient _documentClient, Uri _collectionUri)
    {
      documentClient = _documentClient;
      collectionUri = _collectionUri;
    }


    public async Task<bool> CheckExistsAsync(string reference, ILogger log)
    {
      return await Task.FromResult(true);
    }

    public async Task<IQueryable<T>> Query()
    {
      return documentClient.CreateDocumentQuery<T>(collectionUri);
    }

    public async Task<T> ReadAsync(string reference)
    {
      return await Task.Run(() => Query().Result.SingleOrDefault());
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

    public async Task UpsertAsync(string reference, T _object, ILogger log)
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

    public async Task<bool> DeleteAsync(string reference, ILogger log)
    {
      try
      {
        Task delete = documentClient.DeleteDocumentAsync(reference);
        if (delete.IsCompletedSuccessfully)
        {
          return true;
        }

        return false;
      }
      catch (Exception ex)
      {
        log.LogError($"Delete Object Failed: {ex.Message}");
        return false;
      }
    }
  }
}
