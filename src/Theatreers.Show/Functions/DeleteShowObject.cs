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

namespace Theatreers.Show.Functions
{
  public class DeleteShowObject
  {

    private IShowDomain _showDomain;
    public DeleteShowObject(IShowDomain showDomain)
    {
      _showDomain = showDomain;
    }

    [FunctionAuthorize]
    [FunctionName("DeleteShowObject")]

    public async Task<IActionResult> DeleteShowObjectAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "DELETE",
        Route = "show/{showId}"
      )] HttpRequestMessage req,
      ILogger log,
      ClaimsPrincipal identity,
      string showId
    )
    {

            string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {


                MessageObject<ShowObject> message = new MessageObject<ShowObject>()
        {
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          },
          Body = new ShowObject()
          {
            Doctype = DocTypes.Image,
            Id = showId,
            Partition = showId
          }
        };

        try
        {
          if (await _showDomain.DeleteShowObject(message))
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
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Show Deletion Fail ::  :: Object ID: {showId} - {ex.Message}");
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
