using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Theatreers.Core.Abstractions;
using Microsoft.Azure.Documents;
using Theatreers.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Theatreers.Core.Providers
{/*
  public class CosmosStorageProvider : IStorageProvider
  {
    private IDocumentClient documentClient { get; set; }
    private Uri collectionUri { get; set; }
    public CosmosStorageProvider(IDocumentClient _documentClient, Uri _collectionUri)
    {
      documentClient = _documentClient;
      collectionUri = _collectionUri;
    }

    public IQueryable Query()
    {
      return documentClient.CreateDocumentQuery(collectionUri);
    }

    public async Task CreateAsync(dynamic _object, ILogger log)
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

    public async Task UpdateAsync(dynamic _object, ILogger log)
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
    public async Task UpsertAsync(dynamic _object, ILogger log)
    {
      try {
        await documentClient.UpsertDocumentAsync(collectionUri, _object);
      } catch (Exception ex){
        log.LogError($"Create Object Failed: {ex.Message}");
      }
    }

    public async Task DeleteAsync(string reference, ILogger log)
    {
      try
      {
        await documentClient.DeleteDocumentAsync(reference);
      }
      catch (Exception ex)
      {
        log.LogError($"Delete Object Failed: {ex.Message}");
      }
    }

    public async Task ReadAsync(object id){
      //await Query().Where(doc => doc.id == id);
    }

    public async Task<bool> CheckExistsAsync(string reference, ILogger log)
    {
      return await Task.FromResult(true);
    }
  }*/
}
