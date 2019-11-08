
using Microsoft.AspNetCore.Mvc;
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

namespace Theatreers.Show.Functions
{
    public class CreateShowNewsObject
    {

        private static IShowDomain _showDomain;
        public CreateShowNewsObject(IShowDomain showDomain)
        {
            _showDomain = showDomain;
        }

        [FunctionName("CreateShowNewsObjectByOrchestrator")]
        public async Task<IActionResult> CreateShowNewsObjectByOrchestratorAsync(
          [OrchestrationTrigger] IDurableOrchestrationContext context,
          ILogger log
        )
        {
            //Take the input as a string from the orchestrator function context
            //Deserialize into a transport object
            string rawRequestBody = context.GetInput<string>();
            MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

            try
            {
                await _showDomain.CreateNewsObjectsFromSearch(message, log, 10);
                return new OkResult();
            }
            catch
            {
                return new NotFoundResult();
            }
        }

        [FunctionAuthorize]
        [FunctionName("CreateShowNewsObjectByHttpAsync")]
        public async Task<IActionResult> CreateShowNewsObjectByHttpAsync(
          [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "post",
        Route = "show/{showId}/news"
      )]HttpRequestMessage req,
          ClaimsPrincipal identity,
          string showId,
          ILogger log
            )
        {

            string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {
                NewsObject inputObject = JsonConvert.DeserializeObject<NewsObject>(await req.Content.ReadAsStringAsync());

                MessageObject<NewsObject> message = new MessageObject<NewsObject>()
                {
                    Headers = new MessageHeaders()
                    {
                        RequestCorrelationId = Guid.NewGuid().ToString(),
                        RequestCreatedAt = DateTime.Now
                    },
                    Body = new NewsObject()
                    {
                        CreatedAt = DateTime.Now,
                        Doctype = DocTypes.News,
                        BingId = inputObject.BingId,
                        DatePublished = inputObject.DatePublished,
                        Name = inputObject.Name,
                        Url = inputObject.Url,
                        Partition = showId
                    }
                };

                try
                {
                    await _showDomain.CreateNewsObject(message, log);
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
