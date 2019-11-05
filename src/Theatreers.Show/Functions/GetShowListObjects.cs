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
  public class GetShowListObjects
  {
    private readonly IShowDomain _showDomain;

    public GetShowListObjects(IShowDomain showDomain)
    {
      _showDomain = showDomain;
    }

    [FunctionName("GetShowListAsync")]
    public async Task<IActionResult> GetShowListAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        "get",
        Route = "shows/{letter}"
      )]HttpRequest req,
      ILogger log,
      string letter)
    {
      ICollection<ShowListObject> _object = await _showDomain.GetShowList(letter);

      if (_object != null)
      {
        return new OkObjectResult(_object);
      }

      return new NotFoundObjectResult($"Sorry, but there are no shows that begin with {letter}!");

    }
  }
}