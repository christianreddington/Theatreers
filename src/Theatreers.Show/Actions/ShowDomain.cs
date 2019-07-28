using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Theatreers.Core.Abstractions;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;
using ImageObject = Theatreers.Show.Models.ImageObject;

namespace Theatreers.Show.Actions
{
  public class ShowDomain : IShowDomain
  {
    private readonly IDataLayer _dataLayer;

    public ShowDomain(IDataLayer dataLayer)
    {
      _dataLayer = dataLayer;
    }

    public async Task<ShowObject> GetShow(string id)
    {
      return await _dataLayer.GetShowAsync(id);
    }
    public async Task<ICollection<ShowListObject>> GetShowList(string letter)
    {
      return await _dataLayer.GetShowsAsync(letter);
    }

    public async Task CreateImageObjectsFromSearch(MessageObject<ShowObject> message, ILogger log)
    {
      //Leverage the Cognitive Services Bing Search API and log out the action
      IImageSearchClient client = new ImageSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Searching for associated images");
      Images imageResults = client.Images.SearchAsync(query: $"{message.Body.ShowName} (Musical)").Result;

      //Initialise a temporaryObject and loop through the results
      //For each result, create a new NewsObject which has a condensed set 
      //of properties, for storage in CosmosDB in line with the show data model
      //Once looped through send an OK Result
      //TODO: There is definitely a better way of doing this, but got a rough working approach out
      MessageObject<ImageObject> _object = new MessageObject<ImageObject>()
      {
        Body = new ImageObject()
        {
          CreatedAt = DateTime.Now,
          Doctype = DocTypes.News,
          Partition = message.Body.Partition
        },
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = message.Headers.RequestCorrelationId,
          RequestCreatedAt = DateTime.Now
        }
      };
      foreach (Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models.ImageObject image in imageResults.Value)
      {
        try
        {
          _object.Body = new Models.ImageObject()
          {
            ContentUrl = image.ContentUrl,
            HostPageUrl = image.HostPageUrl,
            ImageId = image.ImageId,
            Name = image.Name
          };

          await _dataLayer.CreateImageAsync(_object);
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Creation Success :: Image ID: {_object.Body.ImageId} ");
        }
        catch (Exception ex)
        {
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Creation Fail ::  :: Image ID: {_object.Body.ImageId} - {ex.Message}");
        }
      }
    }
  }
}
