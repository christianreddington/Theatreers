
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Search;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;

namespace Theatreers.Show
{
    public static class Aggregator
    {

       /* [FunctionName("Aggregator")]
        public static async void Run(
            [ServiceBusTrigger("newshow", "aggregator", Connection = "topicConnectionString")]string topicMessage,
            ILogger log,
            [Blob("showsimage", FileAccess.Read, Connection = "storageConnectionString")] CloudBlobContainer imageBlobContainer,
            [Blob("showsnews", FileAccess.Read, Connection = "storageConnectionString")] CloudBlobContainer newsBlobContainer,
            [CosmosDB(databaseName: "theatreers", collectionName: "items", ConnectionStringSetting = "cosmosConnectionString")] IAsyncCollector<DecoratedShowMessage> reviewOutput
        )
        {
            DecoratedShowMessage decoratedMessage = JsonConvert.DeserializeObject<DecoratedShowMessage>(topicMessage);
            string filename = $"{decoratedMessage.MessageProperties.RequestCorrelationId}.json";
            CloudBlockBlob imageBlob = imageBlobContainer.GetBlockBlobReference(filename);
            CloudBlockBlob newsBlob = newsBlobContainer.GetBlockBlobReference(filename);
            int backoff = 300;

            IList<ImageObject> imageObject = await findBlob<IList<ImageObject>>(imageBlob, backoff);
            IList<NewsObject> newsObject = await findBlob<IList<NewsObject>>(newsBlob, backoff);
            
            decoratedMessage.images = imageObject;
            decoratedMessage.news = newsObject;
            decoratedMessage.doctype = "show";
            decoratedMessage.partitionKey = decoratedMessage.ShowName.ToLower().Substring(0, 4);

            log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: entity downloaded :: {imageObject.FirstOrDefault().ToString()}");
            log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: entity downloaded :: {newsObject.FirstOrDefault().ToString()}");

            try
            {
                await reviewOutput.AddAsync(decoratedMessage);
                await imageBlob.DeleteAsync();
                await newsBlob.DeleteAsync();

                log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Upload Complete {reviewOutput.ToString()}");
                log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Aggregation Clean-up Complete");
            }
            catch (Exception ex)
            {
                log.LogInformation($"[Request Correlation ID: {decoratedMessage.MessageProperties.RequestCorrelationId}] :: Aggregation Clean-up Incomplete :: {ex.Message}");
            }
        }

        public static async Task<T> findBlob<T>(CloudBlockBlob blob, int delay){
            while (!await blob.ExistsAsync()){
                await Task.Delay(delay);
                delay*= 2;
            }

            return JsonConvert.DeserializeObject<T>(await blob.DownloadTextAsync());
        }*/
    }
}