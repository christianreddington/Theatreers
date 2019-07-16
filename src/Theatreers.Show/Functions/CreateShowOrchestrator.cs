//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Theatreers.Show.Models;

//namespace Theatreers.Show.Functions
//{
//  public static class CreateShowOrchestrator
//  {
//    [FunctionName("CreateShowOrchestrator")]
//    public static async Task<IActionResult> CreateShowOrchestratorAsync(
//      [HttpTrigger(
//        AuthorizationLevel.Anonymous,
//        methods: "post",
//        Route = "show"
//      )] HttpRequestMessage req,
//      [OrchestrationClient] DurableOrchestrationClientBase starter,
//      ILogger log,
//      ClaimsPrincipal identity
//    )
//    {
//      if (identity != null && identity.Identity.IsAuthenticated)
//      {
//        //Initialise the message for transport
//        //Generate correllation ID and initial request timestamp
//        string correlationId = Guid.NewGuid().ToString();
//        string showId = Guid.NewGuid().ToString();
//        MessageObject<ShowObject> showObjectInput = new MessageObject<ShowObject>()
//        {
//          Headers = new MessageHeaders()
//          {
//            RequestCorrelationId = correlationId,
//            RequestCreatedAt = DateTime.Now.ToString()
//          },
//          Body = new CosmosBaseObject<ShowObject>()
//          {
//            InnerObject = JsonConvert.DeserializeObject<ShowObject>(await req.Content.ReadAsStringAsync()),
//            ShowId = showId,
//            IsDeleted = false
//          }
//        };
//        string eventData = JsonConvert.SerializeObject(showObjectInput);

//        //Call the downstream "Activity" functions
//        string submitShowInstanceId = await starter.StartNewAsync("CreateShowObject", eventData);
//        string submitNewsInstanceId = await starter.StartNewAsync("CreateShowNewsObject", eventData);
//        string submitImageInstanceId = await starter.StartNewAsync("CreateShowImageObject", eventData);

//        log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowObject instance ID: {submitShowInstanceId}");
//        log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowNewsObject instance ID: {submitNewsInstanceId}");
//        log.LogInformation($"[Request Correlation ID: {correlationId}] :: Begin Orchestration :: CreateShowImageObject instance ID: {submitImageInstanceId}");

//        return new AcceptedResult();
//      }
//      else
//      {
//        return new UnauthorizedResult();
//      }
//    }
//  }
//}
