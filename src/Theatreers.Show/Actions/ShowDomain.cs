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
using Microsoft.Azure.CognitiveServices.Search.NewsSearch;
using NewsApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Search.NewsSearch.ApiKeyServiceClientCredentials;
using ImageApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Search.ImageSearch.ApiKeyServiceClientCredentials;
using Theatreers.Show.Functions;

namespace Theatreers.Show.Actions
{
  public class ShowDomain : IShowDomain
  {
    private readonly IDataLayer _dataLayer;

    public ShowDomain(IDataLayer dataLayer)
    {
      _dataLayer = dataLayer;
    }

    public async Task<ICollection<ImageObject>> GetImageByShow(string id)
    {
      return await _dataLayer.GetImagesByShowAsync(id);
    }

    public async Task<ICollection<NewsObject>> GetNewsByShow(string id)
    {
      return await _dataLayer.GetNewsByShowAsync(id);
    }

    public async Task<ShowObject> GetShow(string id)
    {
      return await _dataLayer.GetShowAsync(id);
    }
    public async Task<ICollection<ShowListObject>> GetShowList(string letter)
    {
      return await _dataLayer.GetShowsAsync(letter);
    }

    public async Task CreateImageObject(MessageObject<ImageObject> message, ILogger log)
    {
      await _dataLayer.CreateImageAsync(message);
    }

    public async Task CreateImageObjectsFromSearch(MessageObject<ShowObject> message, ILogger log, int count = 10)
    {
      //Leverage the Cognitive Services Bing Search API and log out the action
      IImageSearchClient client = new ImageSearchClient(new ImageApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Searching for associated images");
      Images imageResults = client.Images.SearchAsync(query: $"{message.Body.ShowName} (Musical)", count: count).Result;

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
    public async Task CreateNewsObject(MessageObject<NewsObject> message, ILogger log)
    {
      await _dataLayer.CreateNewsAsync(message);
    }

    public async Task CreateNewsObjectsFromSearch(MessageObject<ShowObject> message, ILogger log)
    {
      //Leverage the Cognitive Services Bing Search API and log out the action
      INewsSearchClient client = new NewsSearchClient(new NewsApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Searching for associated images");
      Microsoft.Azure.CognitiveServices.Search.NewsSearch.Models.News newsResults = client.News.SearchAsync(query: $"{message.Body.ShowName} (Musical)").Result;

      //Initialise a temporaryObject and loop through the results
      //For each result, create a new NewsObject which has a condensed set 
      //of properties, for storage in CosmosDB in line with the show data model
      //Once looped through send an OK Result
      //TODO: There is definitely a better way of doing this, but got a rough working approach out
      MessageObject<NewsObject> _object = new MessageObject<NewsObject>()
      {
        Body = new NewsObject()
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
      foreach (Microsoft.Azure.CognitiveServices.Search.NewsSearch.Models.NewsArticle newsItem in newsResults.Value)
      {
        try
        {
          _object.Body = new NewsObject()
          {
            BingId = newsItem.BingId,
            DatePublished = newsItem.DatePublished,
            Name = newsItem.Name,
            Url = newsItem.Url
          };

          await _dataLayer.CreateNewsAsync(_object);
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: News Article Creation Success :: Image ID: {_object.Body.BingId} ");
        }
        catch (Exception ex)
        {
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: News Article Creation Fail ::  :: Image ID: {_object.Body.BingId} - {ex.Message}");
        }
      }
    }
    public async Task CreateShowObject(MessageObject<ShowObject> message)
    {
      await _dataLayer.CreateShowAsync(message);
    }
    public async Task<bool> DeleteImageObject(MessageObject<ImageObject> message)
    {
      return await _dataLayer.DeleteImageAsync(message.Body);
    }
    public async Task<bool> DeleteNewsObject(MessageObject<NewsObject> message)
    {
      return await _dataLayer.DeleteNewsAsync(message.Body);
    }
    public async Task<bool> DeleteShowObject(MessageObject<ShowObject> message)
    {
      return await _dataLayer.DeleteShowAsync(message.Body);
    }
    public async Task<bool> UpdateImageObject(MessageObject<ImageObject> message)
    {
      return await _dataLayer.UpdateImageAsync(message.Body);
    }
    public async Task<bool> UpdateNewsObject(MessageObject<NewsObject> message)
    {
      return await _dataLayer.UpdateNewsAsync(message.Body);
    }
    public async Task<bool> UpdateShowObject(MessageObject<ShowObject> message)
    {
      return await _dataLayer.UpdateShowAsync(message.Body);
    }
  }
}
