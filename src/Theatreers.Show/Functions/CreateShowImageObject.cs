using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;
using Theatreers.Show.Utils;
using System.IO;
using ImageObject = Theatreers.Show.Models.ImageObject;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace Theatreers.Show.Functions
{
    public class CreateShowImageObject
    {

        private static IShowDomain _showDomain;
        public CreateShowImageObject(IShowDomain showDomain)
        {
            _showDomain = showDomain;
        }

        [FunctionName("CreateShowImageObjectByOrchestrator")]

        public async Task<IActionResult> CreateShowImageObjectByOrchestrator(
          [OrchestrationTrigger] IDurableOrchestrationContext context,
          ILogger log,
          [CosmosDB(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString"
      )] IDocumentClient documentClient
        )
        {

            //Take the input as a string from the orchestrator function context
            //Deserialize into a transport object
            string rawRequestBody = context.GetInput<string>();
            MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

            try
            {
                await _showDomain.CreateImageObjectsFromSearch(message, log, 10);
                return new OkResult();
            }
            catch
            {
                return new NotFoundResult();
            }
        }


        [FunctionAuthorize]
        [FunctionName("CreateImageObjectByHttpAsync")]
        public async Task<IActionResult> CreateImageObjectByHttpAsync(
          [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "post",
        Route = "show/{showId}/imageexternal"
      )]HttpRequestMessage req,
          ClaimsPrincipal identity,
          string showId,
          ILogger log
            )
        {
            string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {
                ImageObject inputObject = JsonConvert.DeserializeObject<ImageObject>(await req.Content.ReadAsStringAsync());
                MessageObject<ImageObject> message = new MessageObject<ImageObject>()
                {
                    Headers = new MessageHeaders()
                    {
                        RequestCorrelationId = Guid.NewGuid().ToString(),
                        RequestCreatedAt = DateTime.Now
                    },
                    Body = new ImageObject()
                    {
                        CreatedAt = DateTime.Now,
                        Doctype = DocTypes.News,
                        ContentUrl = inputObject.ContentUrl,
                        HostPageUrl = inputObject.HostPageUrl,
                        ImageId = $"manual-{Guid.NewGuid().ToString()}",
                        Partition = showId
                    }
                };

                try
                {
                    await _showDomain.CreateImageObject(message, log);
                }
                catch (Exception ex)
                {
                    log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of News Article {message.Body.Name} failed :: {ex.Message}");
                    return new BadRequestObjectResult($"There was an error: {ex.Message}");
                }

                log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of News Article {message.Body.Name} succeeded");
                return new OkResult();
            }
            else
            {
                return new UnauthorizedResult();
            }
        }

        [FunctionName("CreateImageObjectOnBlobTrigger")]
        public async void CreateImageObjectOnBlobTrigger(
          [EventGridTrigger]EventGridEvent eventGridEvent,
          ILogger log
            )
        {

            Console.WriteLine(eventGridEvent.Data);
            string GuidAndFilename = eventGridEvent.Subject.Replace("/blobServices/default/containers/show/blobs/", "");
            string[] segments = GuidAndFilename.Split('/');
            string showGuid = segments[0];
            string filename = segments[1];
            object eventData = eventGridEvent.Data;
            string objectJson = JsonConvert.SerializeObject(eventData);
            EventGridSchemaDataObject eventGridSchemaObject = JsonConvert.DeserializeObject<EventGridSchemaDataObject>(objectJson);


            if (eventGridEvent.EventType == "Microsoft.Storage.BlobCreated")
            {
                MessageObject<ImageObject> message = new MessageObject<ImageObject>()
                {
                    Headers = new MessageHeaders()
                    {
                        RequestCorrelationId = Guid.NewGuid().ToString(),
                        RequestCreatedAt = eventGridEvent.EventTime
                    },
                    Body = new ImageObject()
                    {
                        CreatedAt = eventGridEvent.EventTime,
                        Doctype = DocTypes.Image,
                        ContentUrl = eventGridSchemaObject.url,
                        HostPageUrl = "EventGridTrigger",
                        Id = Guid.NewGuid().ToString(),
                        ImageId = filename,
                        Partition = showGuid
                    }
                };

                try
                {
                    await _showDomain.CreateImageObject(message, log);
                }
                catch (Exception ex)
                {
                    log.LogInformation($"Error occured from blob trigger {ex.Message}");
                }
            }
            Console.WriteLine("Hello world");
            /* MessageObject<ImageObject> message = new MessageObject<ImageObject>()
             {
                 Headers = new MessageHeaders()
                 {
                     RequestCorrelationId = Guid.NewGuid().ToString(),
                     RequestCreatedAt = DateTime.Now
                 },
                 Body = new ImageObject()
                 {
                     CreatedAt = DateTime.Now,
                     Doctype = DocTypes.News,
                     ContentUrl = inputObject.ContentUrl,
                     HostPageUrl = inputObject.HostPageUrl,
                     ImageId = $"manual-{Guid.NewGuid().ToString()}",
                     Partition = showId
                 }
             };

             try
             {
                 await _showDomain.CreateImageObject(message, log);
             }
             catch (Exception ex)
             {
                 log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of News Article {message.Body.Name} failed :: {ex.Message}");
                 return new BadRequestObjectResult($"There was an error: {ex.Message}");
             }

             log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of News Article {message.Body.Name} succeeded");
             return new OkResult();*/
        }

        [FunctionAuthorize]
        [FunctionName("UploadImageObjectsToBlobAsync")]
        public async Task<IActionResult> UploadImageObjectsToBlobAsync(
         [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "post",
        Route = "show/{showId}/image"
      )]HttpRequestMessage req,
         ClaimsPrincipal identity,
         string showId,
         ILogger log,
        [Blob("show/{showId}", FileAccess.Write)] Stream image
           )
        {
            string authorizationStatus = req.Headers.GetValues("AuthorizationStatus").FirstOrDefault();
            if (Convert.ToInt32(authorizationStatus).Equals((int)HttpStatusCode.Accepted))
            {

                var provider = new MultipartMemoryStreamProvider();
                await req.Content.ReadAsMultipartAsync(provider);

                List<BlobImagePoco> successfulFiles = new List<BlobImagePoco>();
                List<string> failedFiles = new List<string>();

                foreach (var file in provider.Contents)
                {
                    IImageFormat format = null;
                    var fileData = await file.ReadAsByteArrayAsync();
                    var fileInfo = file.Headers.ContentDisposition;

                    try
                    {
                        using (var tempImage = Image.Load(await file.ReadAsStreamAsync(), out format))
                        {

                        }
                    }
                    catch
                    {
                        failedFiles.Add(fileInfo.FileName);
                        continue;
                    }

                    string filename = $"{showId}/{showId}_{DateTime.Now.Ticks}.{format.Name.ToLower()}";

                    if (!(await _showDomain.SaveToBlobStorage(filename, showId, fileData)))
                    {
                        failedFiles.Add(fileInfo.FileName);
                    }
                    else
                    {
                        successfulFiles.Add(
                            new BlobImagePoco()
                            {
                                url = filename,
                                data = fileInfo.FileName
                            });
                    }
                }

                if (successfulFiles.Count > 0)
                {
                    return new OkObjectResult(successfulFiles);
                }
                else
                {
                    return new BadRequestObjectResult($"{failedFiles.Count} files failed uploading");
                }
            }
            else
            {
                return new UnauthorizedResult();
            }
        }

    }
}
