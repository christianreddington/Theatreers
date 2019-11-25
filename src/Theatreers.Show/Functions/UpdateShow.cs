using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;
using Theatreers.Show.Utils;

namespace Theatreers.Show.Functions
{
    public class UpdateShow
    {
        private readonly IShowDomain _showDomain;

        public UpdateShow(IShowDomain showDomain)
        {
            _showDomain = showDomain;
        }

        [FunctionAuthorize]
        [FunctionName("UpdateShow")]
        public async Task<IActionResult> Run(
          [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "PUT",
        Route = "show/{showId}")] HttpRequestMessage req,
          ILogger log,
          ClaimsPrincipal identity,
          string showId
        )
        {

           // string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
           // if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
           // {
                // Initialise a message object, based upon the information passed into the Microservice
                MessageObject<ShowObject> message = new MessageObject<ShowObject>()
                {
                    Headers = new MessageHeaders()
                    {
                        RequestCorrelationId = Guid.NewGuid().ToString(),
                        RequestCreatedAt = DateTime.Now
                    },
                    Body = JsonConvert.DeserializeObject<ShowObject>(await req.Content.ReadAsStringAsync())
                };
                message.Body.Doctype = DocTypes.Show;
                message.Body.Id = showId;

                // Try the UpdateObject method. If successful, return OK Result. Otherwise, return badrequestresult with an unexpected error.
                // If an exception is generated, throw a BadRequestErrorMessage with the error message.
                try
                {
                    if (await _showDomain.UpdateShowObject(message))
                    {
                        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Show Update Success");
                        return new OkResult();
                    }
                    else
                    {
                        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Show Update Success");
                        return new BadRequestErrorMessageResult("An unexpected error occured");
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Show Update Fail :: {ex.Message}");
                    return new BadRequestErrorMessageResult(ex.Message);
                }
            }
            /*else
            {
                return new UnauthorizedResult();
            }*/
        }
    }
}
