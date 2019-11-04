
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
    public class GetNewsObjectByShow
  {
    private readonly IShowDomain _showDomain;

    public GetNewsObjectByShow(IShowDomain showDomain)
    {
      _showDomain = showDomain;
    }

    [FunctionName("GetNewsObjectByShow")]
    public async Task<IActionResult> GetShowNewsObjectsAsync(
        [HttpTrigger(
          AuthorizationLevel.Anonymous,
          "get",
          Route = "show/{showId}/news")]
        HttpRequest req,
        string showId,
        ILogger log)
    {
      ICollection<NewsObject> _object = await _showDomain.GetNewsByShow(showId);

      if (_object != null && _object.Count > 0)
      {
        return new OkObjectResult(_object);
      }

      JsonResult result = new JsonResult($"Sorry, but the show with ID {showId} does not exist!");
      result.StatusCode = 404;
      return result;
    }
  }
}