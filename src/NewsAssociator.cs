
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Search.NewsSearch;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Theatreers.Show
{
    public static class NewsAssociator
    {
        [FunctionName("NewsAssociator")]
        
        public static void Run(
            [ServiceBusTrigger("newshow", "news", Connection = "topicConnectionString")]string topicMessage,
            ILogger log,
            [Blob("showsnews", FileAccess.Read, Connection = "storageConnectionString")] CloudBlobContainer blobContainer
        )
       
        {
            INewsSearchClient client = new NewsSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
            DecoratedShowMessage decoratedMessage = JsonConvert.DeserializeObject<DecoratedShowMessage>(topicMessage);
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{decoratedMessage.MessageProperties.RequestCorrelationId}.json");

            ShowMessage showMessage = JsonConvert.DeserializeObject<ShowMessage>(topicMessage);
            
            log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Searching for associated news");
            var newsResults = client.News.SearchAsync(query: showMessage.ShowName, market: "en-us", count: 10).Result;

            try {
                blob.UploadTextAsync(JsonConvert.SerializeObject(newsResults.Value));
                log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Image JSON upload completed :: {newsResults.Value.Count} items found");
            } catch(Exception ex) {
                log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Image JSON upload failed :: {ex.Message}");
            }
        }
    }
}
