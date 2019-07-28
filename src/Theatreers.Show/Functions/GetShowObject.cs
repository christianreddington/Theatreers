
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public class GetShowObject
  {
    private readonly IShowDomain _showDomain;

    public GetShowObject(IShowDomain showDomain)
    {
      _showDomain = showDomain;
    }

    [FunctionName("GetShowAsync")]
    public async Task<IActionResult> GetShowAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        "get",
        Route = "show/{id}/show"
      )]HttpRequest req, 
      ILogger log,
      string id)
    {
      ShowObject _object = await _showDomain.GetShow(id);

      if (_object != null)
      {
        return new OkObjectResult(_object);
      }

      return new NotFoundObjectResult($"Sorry, but the show with ID {id} does not exist!");

    }
  }
}