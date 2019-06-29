using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public static class DeleteShowObject
  {
    [FunctionName("DeleteShowObject")]
    public static async Task<IActionResult> DeletedShowObjectAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        "delete",
        Route = "show/{id}"
      )]HttpRequest req,
      ILogger log,
      [CosmosDB(
          databaseName: "theatreers",
          collectionName: "shows",
          ConnectionStringSetting = "cosmosConnectionString"
      )] IDocumentClient documentClient,
      ClaimsPrincipal identity
    )
    {
      if (identity != null && identity.Identity.IsAuthenticated)
      {

        String requestId = req.HttpContext.Request.Path.ToString().Replace("/api/show/", "").Replace("/show", "");

        Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
        CosmosBaseObject<ShowObject> returnedObject = documentClient.CreateDocumentQuery<CosmosBaseObject<ShowObject>>(collectionUri)
                                    .Where(c => c.ShowId == requestId && c.Doctype == "show")
                                    .AsEnumerable()
                                    .FirstOrDefault();

        if (returnedObject != null)
        {
          string CorrelationId = Guid.NewGuid().ToString();
          //Take the input as a string from the orchestrator function context
          //Deserialize into a transport and "returned" object
          //These have subtly different types, the latter having fewer properties for storage in CosmosDB
          returnedObject.Doctype = "show";
          returnedObject.Ttl = 10;
          returnedObject.IsDeleted = true;

          //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
          //If unsuccessful, catch any exception, log that and throw a BadRequestResult
          try
          {
            await documentClient.UpsertDocumentAsync(collectionUri, returnedObject);
            log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: Show Deletion Success");
            return new OkResult();
          }
          catch (Exception ex)
          {
            log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: Show Deletion Fail :: {ex.Message}");
            return new BadRequestResult();
          }
        }
        else
        {
          return new NotFoundResult();
        }
      }
      else
      {
        return new UnauthorizedResult();
      }
    }
  }
}
