
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.MicrosoftGraph;
using Microsoft.Identity.Client;
using System;
using System.Security.Claims;
using System.Threading;
using Theatreers.Admin.Utilities;
using System.Net;
using System.Linq;

namespace Theatreers.Admin
{
    public class GetUsers
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        [FunctionAuthorize]
        [FunctionName("GetUserPermissions")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "moderation/permission")
            ]HttpRequestMessage req,
            ClaimsPrincipal identity,
            TraceWriter log)
        {
            string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {

                string redirectUri = "https://th-admin-dev-weu-func.azurewebsites.net";
                IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(Environment.GetEnvironmentVariable("TheatreersAdminClientId"))
                    .WithTenantId("74b32fe7-9082-4068-9cbe-876773845c52")
                    .WithClientSecret(Environment.GetEnvironmentVariable("TheatreersAdminClientSecret"))
                    .WithRedirectUri(redirectUri)
                    .Build();

                // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the
                // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
                // a tenant administratorResourceId = "someAppIDURI";
                string ResourceId = "https://graph.windows.net";
                string[] scopes = new[] { ResourceId + "/.default" };

                AuthenticationResult token = null;
                try
                {
                    token = await app.AcquireTokenForClient(scopes)
                                      .ExecuteAsync();
                    log.Info($"Token: {token}");
                }
                catch (MsalServiceException ex)
                {
                    // Case when ex.Message contains:
                    // AADSTS70011 Invalid scope. The scope has to be of the form "https://resourceUrl/.default"
                    // Mitigation: change the scope to be as expected
                }

                log.Info($"Token: {token}");
                log.Info("Getting user permissions in B2C Directory");

                string requestBody = await req.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                var request = new HttpRequestMessage();
                log.Info($"Token: {token}");

                request.RequestUri = new System.Uri("https://graph.windows.net/theatreers.onmicrosoft.com/users?api-version=1.6");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
                var client = await httpClient.SendAsync(request);
                var body = await client.Content.ReadAsStringAsync();

                return new OkObjectResult(body);
            }
            else
            {
                return new UnauthorizedResult();
            }
        }
    }
}