using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.WebJobs.ServiceBus;
using System.Collections.Generic;

namespace Theatreers.Show
{
    public static class SubmitShow
    {        
        
        [FunctionName("SubmitShowAsync")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "show")] HttpRequest req,
            ILogger log,
            [ServiceBus("newshow", EntityType.Topic, Connection = "topicConnectionString")] IAsyncCollector<string> outputs
            )
        {
            string rawRequestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string CorrelationId = Guid.NewGuid().ToString();  
            string statusCheckEndpoint = $"{Environment.GetEnvironmentVariable("statusCheckURL")}/{CorrelationId}";

            string showMessageEnvelope = MessageHelper.DecorateJsonBody(rawRequestBody, 
                new Dictionary<string, JToken>(){
                    { "RequestCorrelationId", CorrelationId},
                    { "RequestCreatedAt", DateTime.Now},
                    { "RequestStatus", statusCheckEndpoint}
                }
            );

            await outputs.AddAsync(showMessageEnvelope);
            return new AcceptedResult();
        }
    }
}
