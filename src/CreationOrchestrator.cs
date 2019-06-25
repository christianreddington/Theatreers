using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Theatreers.Show
{
  public static class CreationOrchestrator
  {
    [FunctionName("CreationOrchestrator")]
    public static async Task<HttpResponseMessage> Run(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "post",
        Route = "show"
      )] HttpRequestMessage req,
      [OrchestrationClient] DurableOrchestrationClientBase starter,
      ILogger log,
      ClaimsPrincipal identity
    )
    {
      //  if (identity != null && identity.Identity.IsAuthenticated)
      //  {
      //Initialise the message for transport
      //Generate correllation ID and initial request timestamp
      string correlationId = Guid.NewGuid().ToString();
      string showId = Guid.NewGuid().ToString();
      MessageObject<ShowObject> showObjectInput = new MessageObject<ShowObject>()
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = correlationId,
          RequestCreatedAt = DateTime.Now.ToString()
        },
        Body = new CosmosBaseObject<ShowObject>()
        {
          innerobject = JsonConvert.DeserializeObject<ShowObject>(await req.Content.ReadAsStringAsync()),
          showId = showId,
          isDeleted = "false"
        }
      };
      string eventData = JsonConvert.SerializeObject(showObjectInput);

      //Call the downstream "Activity" functions
      string submitShowInstanceId = await starter.StartNewAsync("CreateShowObject", eventData);
      string submitNewsInstanceId = await starter.StartNewAsync("CreateShowNewsObject", eventData);
      string submitImageInstanceId = await starter.StartNewAsync("CreateShowImageObject", eventData);

      log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowObject instance ID: {submitShowInstanceId}");
      log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowNewsObject instance ID: {submitNewsInstanceId}");
      log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowImageObject instance ID: {submitImageInstanceId}");

      return starter.CreateCheckStatusResponse(req, submitShowInstanceId);
      /*  } else {
             HttpResponseMessage response = new HttpResponseMessage();
             response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
             response.ReasonPhrase = "This is an unauthorized request";
             return response;
         }*/
    }
  }
}
