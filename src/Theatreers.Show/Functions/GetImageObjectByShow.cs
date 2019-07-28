
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Documents;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Extensions.Logging;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Theatreers.Core.Models;
//using Theatreers.Show.Models;

//namespace Theatreers.Show.Functions
//{
//  public static class GetImageObjectByShow
//  {
//    [FunctionName("GetImageObjectByShow")]
//    public static async Task<IActionResult> GetImageObjectByShowAsync(
//        [HttpTrigger(AuthorizationLevel.Anonymous, "get",
//          Route = "show/{id}/image")]HttpRequest req,
//        [CosmosDB(
//          databaseName: "theatreers",
//          collectionName: "shows",
//          ConnectionStringSetting = "cosmosConnectionString",
//          Id = "{id}",
//          PartitionKey = "{id}")] IDocumentClient documentClient,
//        string id,
//        ILogger log)
//    {
//      Actions.Actions action = new Show.Actions.Actions("theatreers", "shows");
//      ICollection<ImageObject> _object = await action.GetImagesByShowAsync(documentClient, id);

//      if (_object != null && _object.Count > 0)
//      {
//        return new OkObjectResult(_object);
//      }

//      return new NotFoundObjectResult($"Sorry, but the show with ID {id} does not exist!");
//    }
//  }
//}