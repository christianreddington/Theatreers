using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Theatreers.Core.Providers;
using Theatreers.Show.Models;
using Theatreers.Core.Abstractions;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Theatreers.Show.Utils;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Actions;
using Theatreers.Show;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

[assembly: FunctionsStartup(typeof(Theatreers.Show.Startup))]

namespace Theatreers.Show
{
  public class Startup : FunctionsStartup
  {
    private static string _databaseId = "theatreers";
    private static string _imageCollectionName = "shows";
    private static string _newsCollectionName = "shows";
    private static string _showCollectionName = "shows";
    private static string _showlistCollectionName = "showlist";
    private static Uri _imageCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _imageCollectionName);
    private static Uri _newsCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _newsCollectionName);
    private static Uri _showCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _showCollectionName);
    private static Uri _showlistCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _showlistCollectionName);
    public override void Configure(IFunctionsHostBuilder builder)
    {
      CosmosDBConnectionString cosmosDBConnectionString = new CosmosDBConnectionString(CosmosDBConnectionString.GetCosmosConnectionString("cosmosConnectionString"));
      IDocumentClient client = new DocumentClient(cosmosDBConnectionString.ServiceEndpoint, cosmosDBConnectionString.AuthKey);
      builder.Services.AddScoped<IStorageProvider<ImageObject>, CosmosStorageProvider<ImageObject>>((s) => { return new CosmosStorageProvider<ImageObject>(client, _imageCollectionUri, _databaseId, _imageCollectionName); });
      builder.Services.AddScoped<IStorageProvider<NewsObject>, CosmosStorageProvider<NewsObject>>((s) => { return new CosmosStorageProvider<NewsObject>(client, _newsCollectionUri, _databaseId, _newsCollectionName); });
      builder.Services.AddScoped<IStorageProvider<ShowObject>, CosmosStorageProvider<ShowObject>>((s) => { return new CosmosStorageProvider<ShowObject>(client, _showCollectionUri, _databaseId, _showCollectionName); });
      builder.Services.AddScoped<IStorageProvider<ShowListObject>, CosmosStorageProvider<ShowListObject>>((s) => { return new CosmosStorageProvider<ShowListObject>(client, _showlistCollectionUri, _databaseId, _showlistCollectionName); });
      builder.Services.AddScoped<IDataLayer, DataLayer>();
      builder.Services.AddScoped<IShowDomain, ShowDomain>();
    }
  }
}
