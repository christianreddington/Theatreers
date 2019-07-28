
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

    [FunctionName("GetImageObjectByShow")]
    public async Task<IActionResult> GetImageObjectByShowAsync(
        [HttpTrigger(
          AuthorizationLevel.Anonymous,
          "get",
          Route = "show/{id}/image")]
        HttpRequest req,
        string id,
        ILogger log)
    {
      ICollection<NewsObject> _object = await _showDomain.GetNewsByShow(id);

      if (_object != null && _object.Count > 0)
      {
        return new OkObjectResult(_object);
      }

      return new NotFoundObjectResult($"Sorry, but the show with ID {id} does not exist!");
    }
  }
}