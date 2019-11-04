using System;
using System.Data.Common;

namespace Theatreers.Show.Utils
{
  public class CosmosDBConnectionString
  {
    public CosmosDBConnectionString(string connectionString)
    {
      // Use this generic builder to parse the connection string
      DbConnectionStringBuilder builder = new DbConnectionStringBuilder
      {
        ConnectionString = connectionString
      };

      if (builder.TryGetValue("AccountKey", out object key))
      {
        AuthKey = key.ToString();
      }

      if (builder.TryGetValue("AccountEndpoint", out object uri))
      {
        ServiceEndpoint = new Uri(uri.ToString());
      }
    }

    public Uri ServiceEndpoint { get; set; }

    public string AuthKey { get; set; }

    public static string GetCosmosConnectionString(string name)
    {
      string conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
      if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
        conStr = System.Environment.GetEnvironmentVariable($"{name}", EnvironmentVariableTarget.Process);
      return conStr;
    }
  }
}
