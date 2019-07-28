
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Linq;
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
      if (identity != null && identity.Identity.IsAuthenticated)
      {
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
            if (await _showDomain.DeleteImageObject(message)){
              return new OkResult();
            } else
            {
              return new NotFoundResult();
            }
          }
          catch (Exception ex)
          {
            log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Deletion Fail ::  :: Object ID: {imageId} - {ex.Message}");
            return new BadRequestResult();
          }
        }
        else
        {
          return new UnauthorizedResult();
        }
    }
  }
}
