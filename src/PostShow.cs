using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;

namespace Theatreers.Show
{
    public static class PostShow
    {        
        [FunctionName("PostShow")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "post", Route = "show")] HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClientBase starter,
            ILogger log)
        {
           // if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity.IsAuthenticated)
          //  {
                //Initialise the message for transport
                //Generate correllation ID and initial request timestamp
                string CorrelationId = Guid.NewGuid().ToString();
                string showId = Guid.NewGuid().ToString();
                DecoratedShowMessage showObjectInput = await req.Content.ReadAsAsync<DecoratedShowMessage>();
                MessageHeaders messageHeaders = new MessageHeaders();
                messageHeaders.RequestCorrelationId = CorrelationId;
                messageHeaders.RequestCreatedAt = DateTime.Now.ToString();
                showObjectInput.MessageProperties = messageHeaders;
                showObjectInput.showId = showId;
                string eventData = JsonConvert.SerializeObject(showObjectInput);

                //Call the downstream "Activity" functions
                string submitShowInstanceId = await starter.StartNewAsync("SubmitShowAsync", eventData);
                string submitNewsInstanceId = await starter.StartNewAsync("SubmitNewsAsync", eventData);
                string submitImageInstanceId = await starter.StartNewAsync("SubmitImageAsync", eventData);

                log.LogInformation($"[Request Correlation ID: {messageHeaders.RequestCorrelationId}] :: Begin Orchestration :: SubmitShowAsync instance ID: {submitShowInstanceId}");
                log.LogInformation($"[Request Correlation ID: {messageHeaders.RequestCorrelationId}] :: Begin Orchestration :: SubmitNewsAsync instance ID: {submitNewsInstanceId}");
                log.LogInformation($"[Request Correlation ID: {messageHeaders.RequestCorrelationId}] :: Begin Orchestration :: SubmitImageAsync instance ID: {submitImageInstanceId}");

                return starter.CreateCheckStatusResponse(req, submitShowInstanceId);
           /* } else
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                response.ReasonPhrase = "The user is not logged in";
                return response;
            }*/
        }
    }
}
