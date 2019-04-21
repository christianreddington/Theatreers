
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
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Theatreers.Show
{
    public static class ImageAssociator
    {
        [FunctionName("ImageAssociator")]
        
        public static void Run(
            [ServiceBusTrigger("newshow", "image", Connection = "topicConnectionString")]string topicMessage, 
            ILogger log,
            [Blob("showsimage", FileAccess.Read, Connection = "storageConnectionString")] CloudBlobContainer blobContainer
        )
        {
            IImageSearchClient client = new ImageSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
            DecoratedShowMessage decoratedMessage = JsonConvert.DeserializeObject<DecoratedShowMessage>(topicMessage);
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{decoratedMessage.MessageProperties.RequestCorrelationId}.json");

            log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Searching for associated images");
            var imageResults = client.Images.SearchAsync(query: decoratedMessage.ShowName).Result;            

            try {
                blob.UploadTextAsync(JsonConvert.SerializeObject(imageResults.Value));
                log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Image JSON upload completed :: {imageResults.Value.Count} items found");
            } catch(Exception ex) {
                log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Image JSON upload failed :: {ex.Message}");
            }
        }
    }
}
