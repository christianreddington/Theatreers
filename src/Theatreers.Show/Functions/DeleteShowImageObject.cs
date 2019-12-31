using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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
    public class DeleteShowImageObject
    {

        private IShowDomain _showDomain;
        public DeleteShowImageObject(IShowDomain showDomain)
        {
            _showDomain = showDomain;
        }

        [FunctionAuthorize]
        [FunctionName("DeleteShowImageObject")]

        public async Task<IActionResult> DeleteShowImageObjectAsync(
          [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "DELETE",
        Route = "show/{showId}/image/{imageId}"
      )] HttpRequestMessage req,
          ILogger log,
          ClaimsPrincipal identity,
          string showId,
          string imageId
        )
        {

            /*string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {*/
                MessageObject<ImageObject> message = new MessageObject<ImageObject>()
                {
                    Headers = new MessageHeaders()
                    {
                        RequestCorrelationId = Guid.NewGuid().ToString(),
                        RequestCreatedAt = DateTime.Now
                    },
                    Body = new ImageObject()
                    {
                        Doctype = DocTypes.Image,
                        Id = imageId,
                        Partition = showId
                    }
                };

                try
                {
                    if (await _showDomain.DeleteImageObject(message))
                    {
                        return new OkResult();
                    }
                    else
                    {
                        return new NotFoundResult();
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Deletion Fail ::  :: Object ID: {imageId} - {ex.Message}");
                    return new BadRequestResult();
                }
            /*}
            else
            {
                return new UnauthorizedResult();
            }*/
        }
    }
}
