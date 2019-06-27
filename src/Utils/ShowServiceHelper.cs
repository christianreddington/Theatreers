using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Theatreers.Show.Models;

namespace Theatreers.Show.Utils
{

  public class ShowServiceHelper
  {
    public static async Task<IActionResult> GetObjectsAsync(
    string objectName,
    HttpRequest req,
    IDocumentClient documentClient,
    ILogger log,
    string collectionName,
    string endpointName
    )
    {
      dynamic results;
      string CorrelationId = Guid.NewGuid().ToString();
      String requestId = req.HttpContext.Request.Path.ToString().Replace($"/api/{endpointName}/", "").Replace($"/{objectName}", "");

      Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", collectionName);
      if (objectName != "showList")
      {
        results = documentClient.CreateDocumentQuery<CosmosBaseObject<dynamic>>(collectionUri)
                                  .Where(c => c.ShowId == requestId && c.Doctype == objectName)
                                  .AsEnumerable();
      } else
      {
        results = documentClient.CreateDocumentQuery<CosmosBaseObject<ShowListObject>>(collectionUri, new FeedOptions { PartitionKey = new PartitionKey(requestId) })
                                  .Where(c => c.InnerObject.Partition == requestId)
                                  .AsEnumerable();
      }

      if (results == null)
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: {objectName} API Request failure :: ID {requestId}");
        return new NotFoundResult();
      }
      else
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: {objectName} API Request success :: ID {requestId}");
        return new OkObjectResult(results);
      }
    }
  }
}