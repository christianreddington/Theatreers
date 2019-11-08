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
using Theatreers.Show.Models;
using Theatreers.Show.Utils;

namespace Theatreers.Show.Functions
{
    public static class CreateShowOrchestrator
    {
        [FunctionAuthorize]
        [FunctionName("CreateShowOrchestrator")]
        public static async Task<IActionResult> CreateShowOrchestratorAsync(
          [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "post",
        Route = "show"
      )] HttpRequestMessage req,
          [DurableClient] IDurableOrchestrationClient starter,
          ILogger log,
          ClaimsPrincipal identity
        )
        {

            string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {
                //Initialise the message for transport
                //Generate correllation ID and initial request timestamp
                string correlationId = Guid.NewGuid().ToString();
                string showId = Guid.NewGuid().ToString();
                MessageObject<ShowObject> showObjectInput = new MessageObject<ShowObject>()
                {
                    Headers = new MessageHeaders()
                    {
                        RequestCorrelationId = correlationId,
                        RequestCreatedAt = DateTime.Now
                    },
                    Body = JsonConvert.DeserializeObject<ShowObject>(await req.Content.ReadAsStringAsync())
                };

                showObjectInput.Body.Id = showId;
                showObjectInput.Body.Partition = showId;
                showObjectInput.Body.IsDeleted = false;

                string eventData = JsonConvert.SerializeObject(showObjectInput);

                //Call the downstream "Activity" functions
                string submitShowInstanceId = await starter.StartNewAsync("CreateShowObjectByOrchestrator", eventData);
                string submitNewsInstanceId = await starter.StartNewAsync("CreateShowNewsObjectByOrchestrator", eventData);
                string submitImageInstanceId = await starter.StartNewAsync("CreateShowImageObjectByOrchestrator", eventData);

                log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowObject instance ID: {submitShowInstanceId}");
                log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowNewsObject instance ID: {submitNewsInstanceId}");
                log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowImageObject instance ID: {submitImageInstanceId}");

                return new AcceptedResult();
            }
            else
            {
                return new UnauthorizedResult();
            }
        }
    }
}
