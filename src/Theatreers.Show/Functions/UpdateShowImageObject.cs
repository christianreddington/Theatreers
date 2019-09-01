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
using ImageObject = Theatreers.Show.Models.ImageObject;

namespace Theatreers.Show.Functions
{
    public class UpdateShowImageObject
    {
        private readonly IShowDomain _showDomain;

        public UpdateShowImageObject(IShowDomain showDomain)
        {
            _showDomain = showDomain;
        }
        [FunctionAuthorize]
        [FunctionName("UpdateShowImageObject")]

        public async Task<IActionResult> UpdateShowImageObjectAsync(
          [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "PUT",
        Route = "show/{showId}/image/{imageId}"
      )] HttpRequestMessage req,
          ILogger log,
          ClaimsPrincipal identity,
          string showId,
          string imageId
        )
        {

            string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {

                // Initialise a message object, based upon the information passed into the Microservice
                MessageObject<ImageObject> message = new MessageObject<ImageObject>()
                {
                    Headers = new MessageHeaders()
                    {
                        RequestCorrelationId = Guid.NewGuid().ToString(),
                        RequestCreatedAt = DateTime.Now
                    },
                    Body = JsonConvert.DeserializeObject<ImageObject>(await req.Content.ReadAsStringAsync())
                };
                message.Body.Partition = showId;
                message.Body.Doctype = DocTypes.Image;
                message.Body.Id = imageId;

                // Try the UpdateObject method. If successful, return OK Result. Otherwise, return badrequestresult with an unexpected error.
                // If an exception is generated, throw a BadRequestErrorMessage with the error message.
                try
                {
                    if (await _showDomain.UpdateImageObject(message))
                    {
                        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Update Success");
                        return new OkResult();
                    }
                    else
                    {
                        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Update Success");
                        return new BadRequestErrorMessageResult("An unexpected error occured");
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Update Fail :: {ex.Message}");
                    return new BadRequestErrorMessageResult(ex.Message);
                }
            }
            else
            {
                return new UnauthorizedResult();
            }
        }
    }
}
