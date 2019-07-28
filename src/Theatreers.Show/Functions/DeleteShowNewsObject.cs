using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public class DeleteShowNewsObject
  {

    private IShowDomain _showDomain;
    public DeleteShowNewsObject(IShowDomain showDomain)
    {
      _showDomain = showDomain;
    }

    [FunctionName("DeleteShowNewsObject")]

    public async Task<IActionResult> DeleteShowNewsObjectAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "DELETE",
        Route = "show/{showId}/news/{newsId}"
      )] HttpRequestMessage req,
      ILogger log,
      ClaimsPrincipal identity,
      string showId,
      string newsId
    )
    {
      if (identity != null && identity.Identity.IsAuthenticated)
      {


        MessageObject<NewsObject> message = new MessageObject<NewsObject>()
        {
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          },
          Body = new NewsObject()
          {
            Doctype = DocTypes.Image,
            Id = newsId,
            Partition = showId
          }
        };

        try
        {
          if (await _showDomain.DeleteNewsObject(message))
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
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Deletion Fail ::  :: Object ID: {newsId} - {ex.Message}");
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
