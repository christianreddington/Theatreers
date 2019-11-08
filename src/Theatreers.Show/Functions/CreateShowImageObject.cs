using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;
using Theatreers.Show.Utils;
using ImageObject = Theatreers.Show.Models.ImageObject;

namespace Theatreers.Show.Functions
{
    public class CreateShowImageObject
    {

        private static IShowDomain _showDomain;
        public CreateShowImageObject(IShowDomain showDomain)
        {
            _showDomain = showDomain;
        }

        [FunctionName("CreateShowImageObjectByOrchestrator")]

        public async Task<IActionResult> CreateShowImageObjectByOrchestrator(
          [OrchestrationTrigger] IDurableOrchestrationContext context,
          ILogger log,
          [CosmosDB(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString"
      )] IDocumentClient documentClient
        )
        {

            //Take the input as a string from the orchestrator function context
            //Deserialize into a transport object
            string rawRequestBody = context.GetInput<string>();
            MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

            try
            {
                await _showDomain.CreateImageObjectsFromSearch(message, log, 10);
                return new OkResult();
            }
            catch
            {
                return new NotFoundResult();
            }
        }


        [FunctionAuthorize]
        [FunctionName("CreateImageObjectByHttpAsync")]
        public async Task<IActionResult> CreateImageObjectByHttpAsync(
          [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "post",
        Route = "show/{showId}/image"
      )]HttpRequestMessage req,
          ClaimsPrincipal identity,
          string showId,
          ILogger log
            )
        {
            string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {
                ImageObject inputObject = JsonConvert.DeserializeObject<ImageObject>(await req.Content.ReadAsStringAsync());
                MessageObject<ImageObject> message = new MessageObject<ImageObject>()
                {
                    Headers = new MessageHeaders()
                    {
                        RequestCorrelationId = Guid.NewGuid().ToString(),
                        RequestCreatedAt = DateTime.Now
                    },
                    Body = new ImageObject()
                    {
                        CreatedAt = DateTime.Now,
                        Doctype = DocTypes.News,
                        ContentUrl = inputObject.ContentUrl,
                        HostPageUrl = inputObject.HostPageUrl,
                        ImageId = $"manual-{Guid.NewGuid().ToString()}",
                        Partition = showId
                    }
                };

                try
                {
                    await _showDomain.CreateImageObject(message, log);
                }
                catch (Exception ex)
                {
                    log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of News Article {message.Body.Name} failed :: {ex.Message}");
                    return new BadRequestObjectResult($"There was an error: {ex.Message}");
                }

                log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of News Article {message.Body.Name} succeeded");
                return new OkResult();
            }
            else
            {
                return new UnauthorizedResult();
            }
        }
    }
}
