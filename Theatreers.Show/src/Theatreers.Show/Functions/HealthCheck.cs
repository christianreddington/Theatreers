
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
  public class HealthCheck
    {
    private readonly IShowDomain _showDomain;

    public HealthCheck(IShowDomain showDomain)
    {
      _showDomain = showDomain;
    }

    [FunctionName("HealthCheckProbe")]
    public async Task<IActionResult> GetShowAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        "get",
        Route = "health"
      )]HttpRequest req, 
      ILogger log)
    {
        return new OkResult();
    }
  }
}