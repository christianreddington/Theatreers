
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public class GetImageObjectByShow
  {
    private readonly IShowDomain _showDomain;

    public GetImageObjectByShow(IShowDomain showDomain)
    {
      _showDomain = showDomain;
    }

    [FunctionName("GetImageObjectsByShow")]
    public async Task<IActionResult> GetImageObjectByShowAsync(
        [HttpTrigger(
          AuthorizationLevel.Anonymous,
          "get",
          Route = "show/{showId}/image")]
        HttpRequest req,
        string showId,
        ILogger log)
    {
      ICollection<ImageObject> _object = await _showDomain.GetImageByShow(showId);

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