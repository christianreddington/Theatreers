using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Theatreers.Core.Abstractions;
using Theatreers.Show.Abstractions;
using Theatreers.Show.Models;
using Theatreers.Core.Providers;
using Xunit;
using Microsoft.Azure.Documents;
using Theatreers.Core.Models;
using Theatreers.Show.Actions;
using System.Collections.Generic;
using System.Threading;
using System.Collections;

namespace Theatreers.Show.Test
{
  public class ShowDomainTests : IAsyncLifetime
  {
    private static string _databaseId = "theatreers";
    private static string _imageCollectionName = "shows";
    private static string _newsCollectionName = "shows";
    private static string _showCollectionName = "shows";
    private static string _showlistCollectionName = "showlist";
    public static Uri _imageCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _imageCollectionName);
    private static Uri _newsCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _newsCollectionName);
    private static Uri _showCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _showCollectionName);
    private static Uri _showlistCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _showlistCollectionName);
    private static IShowDomain _showDomain;
    private static List<string> ids = new List<string>() { "1", "2", "3", "4" };


    public async Task InitializeAsync()
    {
      IDocumentClient client = new DocumentClient(new System.Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
      IStorageProvider<ImageObject> _imageStore = new CosmosStorageProvider<ImageObject>(client, _imageCollectionUri, _databaseId, _imageCollectionName);
      IStorageProvider<NewsObject> _newsStore = new CosmosStorageProvider<NewsObject>(client, _newsCollectionUri, _databaseId, _newsCollectionName);
      IStorageProvider<ShowObject> _showStore = new CosmosStorageProvider<ShowObject>(client, _showCollectionUri, _databaseId, _showCollectionName);
      IStorageProvider<ShowListObject> _showListStore = new CosmosStorageProvider<ShowListObject>(client, _showlistCollectionUri, _databaseId, _showlistCollectionName);
      IDataLayer _dataLayer = new DataLayer(_imageStore, _newsStore, _showStore, _showListStore);
      _showDomain = new ShowDomain(_dataLayer);

      ILogger log = new StubLogger();

      string databaseName = "theatreers";
      string collectionName = "shows";

      IDocumentClient documentClient = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
      Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
      Uri databaseUri = UriFactory.CreateDatabaseUri(databaseName);

      Database theatreersDatabase = new Database()
      {
        Id = databaseName
      };

      await documentClient.CreateDatabaseIfNotExistsAsync(theatreersDatabase);

      DocumentCollection showCollection = new DocumentCollection()
      {
        Id = collectionName
      };

      showCollection.PartitionKey.Paths.Add("/partition");
      showCollection.DefaultTimeToLive = -1;
      await documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseUri, showCollection);


      foreach (string id in ids)
      {
        await _showDomain.CreateShowObject(new MessageObject<ShowObject>()
        {
          Body = new ShowObject()
          {
            Id = id,
            Partition = id,
            Doctype = DocTypes.Show,
            ShowName = $"Some Show Name #{id}"
          },
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          }
        });
        int childRecords = 0;
        Int32.TryParse(id, out childRecords);
        childRecords *= childRecords;

        for (int i = 0; i < childRecords; i++)
        {
          await _showDomain.CreateImageObject(new MessageObject<ImageObject>()
          {
            Body = new ImageObject()
            {
              Id = $"i{i}",
              Partition = id,
              Doctype = DocTypes.Image,
              ContentUrl = "https://localhost/image.jpg",
              Name = "Example Image"
            },
            Headers = new MessageHeaders()
            {
              RequestCorrelationId = Guid.NewGuid().ToString(),
              RequestCreatedAt = DateTime.Now
            }
          }, log);


          await _showDomain.CreateNewsObject(new MessageObject<NewsObject>()
          {
            Body = new NewsObject()
            {
              Id = $"n{i}",
              Partition = id,
              Doctype = DocTypes.News,
              DatePublished = DateTime.Now.ToString(),
              Url = "http://somenewssite.com",
              Name = "Some News Article"
            },
            Headers = new MessageHeaders()
            {
              RequestCorrelationId = Guid.NewGuid().ToString(),
              RequestCreatedAt = DateTime.Now
            }
          }, log);
        }
      }
    }

    public async Task DisposeAsync()
    {
      string databaseName = "theatreers";
      string collectionName = "shows";

      IDocumentClient documentClient = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
      Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
      Uri databaseUri = UriFactory.CreateDatabaseUri(databaseName);

      await documentClient.DeleteDatabaseAsync(databaseUri);
    }


    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task CheckImagesExistByShowIdThatExists(string showId)
    {
      // Arrange
      int idAsInteger = 0;
      int expectedCount = 0;
      Int32.TryParse(showId, out idAsInteger);
      expectedCount = idAsInteger * idAsInteger;

      // Act  
      ICollection<ImageObject> imageObjects = await _showDomain.GetImageByShow(showId);

      //Assert
      Assert.Equal(expectedCount, imageObjects.Count);
    }


    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task GetImageObjectByShowSucceedsForValidId(string showId)
    {
      // Arrange
      int idAsInteger = 0;
      int expectedCount = 0;
      Int32.TryParse(showId, out idAsInteger);
      expectedCount = idAsInteger * idAsInteger;

      // Act  
      ICollection<NewsObject> newsObjects = await _showDomain.GetNewsByShow(showId);

      //Assert
      Assert.Equal(expectedCount, newsObjects.Count);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task GetNewsObjectByShowSucceedsForValidId(string showId)
    {
      // Arrange
      int idAsInteger = 0;
      int expectedCount = 0;
      Int32.TryParse(showId, out idAsInteger);
      expectedCount = idAsInteger * idAsInteger;

      // Act  
      ICollection<NewsObject> newsObjects = await _showDomain.GetNewsByShow(showId);

      //Assert
      Assert.Equal(expectedCount, newsObjects.Count);
    }


    [Fact]
    public async Task GetNewsObjectByShowReturnsZeroForNonExistingId()
    {
      string showId = "5";

      // Act  
      ICollection<NewsObject> newsObjects = await _showDomain.GetNewsByShow(showId);

      //Assert
      Assert.Equal(0, newsObjects.Count);
    }

    [Fact]
    public async Task GetImageObjectByShowReturnsZeroForNonExistingId()
    {
      string showId = "5";

      // Act  
      ICollection<ImageObject> imageObjects = await _showDomain.GetImageByShow(showId);

      //Assert
      Assert.Equal(0, imageObjects.Count);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task GetShowSucceedsWithValidId(string showId)
    {
      // Arrange
      int idAsInteger = 0;
      Int32.TryParse(showId, out idAsInteger);

      // Act  
      ShowObject showObject = await _showDomain.GetShow(showId);

      //Assert
      Assert.NotNull(showObject);
    }



    [Theory]
    [InlineData("P")]
    [InlineData("J")]
    public async Task GetShowListSucceedsWithValidPartition(string partition)
    {
      // Act  
      ICollection<ShowListObject> showList = await _showDomain.GetShowList(partition);

      //Assert
      Assert.NotNull(showList);
      Assert.Equal(1, showList.Count);
    }

    [Fact]
    public async Task GetShowListReturnzZeroWithNonExistingPartition()
    {
      string partition = "NotAPartition";
      // Act  
      ICollection<ShowListObject> showList = await _showDomain.GetShowList(partition);

      //Assert
      Assert.NotNull(showList);
      Assert.Equal(0, showList.Count);
    }

    [Fact]
    public async Task GetShowReturnsNullWithInvalidId()
    {
      string showId = "5";

      // Act  
      ShowObject showObject = await _showDomain.GetShow(showId);

      //Assert
      Assert.Null(showObject);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task DeleteImageObjectSucceeds(string showId)
    {
      int idAsInteger = 0;
      Int32.TryParse(showId, out idAsInteger);
      int numberOfChildObjects = idAsInteger * idAsInteger;

      // Act
      for (int i = 0; i < numberOfChildObjects; i++)
      {
        ImageObject imageObject = new ImageObject()
        {
          Id = $"i{i}",
          Partition = showId,
          Doctype = DocTypes.Image,
          Name = $"Example Show {showId}",
          ContentUrl = "http://localhost.com"
        };

        MessageObject<ImageObject> message = new MessageObject<ImageObject>
        {
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          },
          Body = imageObject
        };

        // Act
        Assert.True(await _showDomain.DeleteImageObject(message));
      }

      // Assert
      Thread.Sleep(2000);
      Assert.Empty(await _showDomain.GetImageByShow(showId));
    }

    [Fact]
    public async Task DeleteImageObjectFailsWhenIdAndPartitionNotValid()
    {

      string imageId = "100";
      string showId = "100";

      ImageObject imageObject = new ImageObject()
      {
        Id = $"i{imageId}",
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = $"Example Show {showId}",
        ContentUrl = "http://localhost.com"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      Assert.False(await _showDomain.DeleteImageObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Empty(await _showDomain.GetImageByShow(showId));
    }


    [Fact]
    public async Task DeleteImageObjectThrowsWhenMissingId()
    {
      string showId = "1";

      ImageObject imageObject = new ImageObject()
      {
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = $"Example Show {showId}",
        ContentUrl = "http://localhost.com"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.DeleteImageObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Equal(1, (await _showDomain.GetImageByShow(showId)).Count);
    }

    [Fact]
    public async Task DeleteImageObjectThrowsWhenMissingPartition()
    {
      string imageId = "1";
      string showId = "1";

      ImageObject imageObject = new ImageObject()
      {
        Id = $"i{imageId}",
        Doctype = DocTypes.Image,
        Name = $"Example Show {showId}",
        ContentUrl = "http://localhost.com"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.DeleteImageObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Equal(1, (await _showDomain.GetImageByShow(showId)).Count);
    }

    [Fact]
    public async Task DeleteImageObjectFailsWhenIdNotValidButPartitionIsValid()
    {

      string imageId = "100";
      string showId = "1";

      ImageObject imageObject = new ImageObject()
      {
        Id = $"i{imageId}",
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = $"Example Show {showId}",
        ContentUrl = "http://localhost.com"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      Assert.False(await _showDomain.DeleteImageObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Equal(1, (await _showDomain.GetImageByShow(showId)).Count);
    }

    [Fact]
    public async Task DeleteImageObjectFailsWhenPartitionNotValid()
    {

      string imageId = "i1";
      string showId = "5";

      ImageObject imageObject = new ImageObject()
      {
        Id = $"i{imageId}",
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = $"Example Show {showId}",
        ContentUrl = "http://localhost.com"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      Assert.False(await _showDomain.DeleteImageObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Empty(await _showDomain.GetImageByShow(showId));
    }

    [Fact]
    public async Task DeleteShowFailsWhenObjectDoesntExist()
    {
      string showId = "5";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        Doctype = DocTypes.Show,
        ShowName = $"Example Show {showId}"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };
      // Arrange
      int idAsInteger = 0;
      Int32.TryParse(showId, out idAsInteger);



      // Act
      var deleteSucceeds = await _showDomain.DeleteShowObject(message);

      // Assert
      Thread.Sleep(2000);
      Assert.False(deleteSucceeds);
      Assert.Null(await _showDomain.GetShow(showId));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task DeleteShowSucceeds(string showId)
    {
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        Doctype = DocTypes.Show,
        ShowName = $"Example Show {showId}"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };
      // Arrange
      int idAsInteger = 0;
      Int32.TryParse(showId, out idAsInteger);

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.DeleteShowObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Null(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }

    [Fact]
    public async Task DeleteShowThrowsWhenMissingId()
    {
      string showId = "1";
      ShowObject showObject = new ShowObject()
      {
        Partition = showId,
        Doctype = DocTypes.Show,
        ShowName = $"Example Show {showId}"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.DeleteShowObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.NotNull(await _showDomain.GetShow(showId));
    }


    [Fact]
    public async Task DeleteShowThrowsWhenMissingPartition()
    {
      string showId = "1";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Doctype = DocTypes.Show,
        ShowName = $"Example Show {showId}"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.DeleteShowObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.NotNull(await _showDomain.GetShow(showId));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task DeleteNewsObjectSucceeds(string showId)
    {
      int idAsInteger = 0;
      Int32.TryParse(showId, out idAsInteger);
      int numberOfChildObjects = idAsInteger * idAsInteger;

      // Act
      for (int i = 0; i < numberOfChildObjects; i++)
      {
        NewsObject newsObject = new NewsObject()
        {
          Id = $"n{i}",
          Partition = showId,
          Doctype = DocTypes.News,
          Name = $"Example Show {showId}",
          DatePublished = DateTime.Now.ToString(),
          Url = "http://localhost"
        };

        MessageObject<NewsObject> message = new MessageObject<NewsObject>
        {
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          },
          Body = newsObject
        };

        // Act
        Assert.True(await _showDomain.DeleteNewsObject(message));
      }

      // Assert
      Thread.Sleep(2000);
      Assert.Empty(await _showDomain.GetNewsByShow(showId));
    }

    [Fact]
    public async Task DeleteNewsObjectThrowsWhenMissingId()
    {
      string showId = "1";

      NewsObject newsObject = new NewsObject()
      {
        Partition = showId,
        Doctype = DocTypes.News,
        Name = $"Example Show {showId}",
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost.com"
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.DeleteNewsObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Equal(1, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task DeleteNewsObjectThrowsWhenMissingPartition()
    {
      string newsId = "1";
      string showId = "1";

      NewsObject newsObject = new NewsObject()
      {
        Id = $"n{newsId}",
        Doctype = DocTypes.News,
        Name = $"Example Show {showId}",
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost.com"
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.DeleteNewsObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Equal(1, (await _showDomain.GetNewsByShow(showId)).Count);
    }


    [Fact]
    public async Task DeleteNewsObjectFailsWhenIdAndPartitionNotValid()
    {

      string newsId = "100";
      string showId = "100";

      NewsObject newsObject = new NewsObject()
      {
        Id = $"n{newsId}",
        Partition = showId,
        Doctype = DocTypes.News,
        Name = $"Example Show {showId}",
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost"
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      Assert.False(await _showDomain.DeleteNewsObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Empty(await _showDomain.GetNewsByShow(showId));
    }

    [Fact]
    public async Task DeleteNewsObjectFailsWhenIdNotValidButPartitionIsValid()
    {

      string newsId = "100";
      string showId = "1";

      NewsObject newsObject = new NewsObject()
      {
        Id = $"n{newsId}",
        Partition = showId,
        Doctype = DocTypes.News,
        Name = $"Example Show {showId}",
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost"
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };


      // Act
      Assert.False(await _showDomain.DeleteNewsObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Equal(1, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task DeleteNewsObjectFailsWhenPartitionNotValid()
    {

      string newsId = "1";
      string showId = "5";


      NewsObject newsObject = new NewsObject()
      {
        Id = $"n{newsId}",
        Partition = showId,
        Doctype = DocTypes.News,
        Name = $"Example Show {showId}",
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost"
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      Assert.False(await _showDomain.DeleteNewsObject(message));

      // Assert
      Thread.Sleep(2000);
      Assert.Empty(await _showDomain.GetNewsByShow(showId));
    }

    [Theory]
    [InlineData("5")]
    [InlineData("6")]
    [InlineData("7")]
    [InlineData("8")]
    public async Task CheckShowThatDoesntExistReturnsNull(string showId)
    {
      // Arrange
      int idAsInteger = 0;
      Int32.TryParse(showId, out idAsInteger);

      // Act  
      ShowObject showObject = await _showDomain.GetShow(showId);

      //Assert
      Assert.Null(showObject);
    }

    [Fact]
    public async Task CanCreateImageWithValidData()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      ImageObject imageObject = new ImageObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = "Example Image",
        ContentUrl = "http://localhost/something.jpg"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateImageObject(message, log));

      // Assert
      Assert.Null(ex);
      Assert.Equal(1, (await _showDomain.GetImageByShow(showId)).Count);
    }


    [Fact]
    public async Task CanCreateShowWithValidData()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        Doctype = DocTypes.Show,
        ShowName = "Phantom of the Opera"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateShowObject(message));

      // Assert
      Assert.Null(ex);
      Assert.NotNull(await _showDomain.GetShow(showId));
    }


    [Fact]
    public async Task CannotCreateShowObjectWithoutId()
    {
      String showId = "5";
      ShowObject showObject = new ShowObject()
      {
        Partition = showId,
        Doctype = DocTypes.Show,
        ShowName = "Phantom of the Opera"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateShowObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }

    [Fact]
    public async Task CannotCreateShowObjectWithoutPartition()
    {
      String showId = "5";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Doctype = DocTypes.Show,
        ShowName = "Phantom of the Opera"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateShowObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }

    [Fact]
    public async Task CannotCreateShowObjectWithoutDoctype()
    {
      String showId = "5";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        ShowName = "Phantom of the Opera"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateShowObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }

    [Fact]
    public async Task CannotCreateShowObjectWithoutShowname()
    {
      String showId = "5";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        Doctype = DocTypes.Show
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateShowObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }

    [Fact]
    public async Task CanCreateNewsObjectWithValidData()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      NewsObject newsObject = new NewsObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Doctype = DocTypes.News,
        Name = "Example Image",
        Url = "http://somenewswebsite.co.uk/news",
        DatePublished = DateTime.Now.ToString()
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateNewsObject(message, log));

      // Assert
      Assert.Null(ex);
      Assert.Equal(1, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task CannotCreateNewsObjectWithoutId()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      NewsObject newsObject = new NewsObject()
      {
        Partition = showId,
        Doctype = DocTypes.News,
        Name = "Example Image",
        Url = "http://somenewswebsite.co.uk/news",
        DatePublished = DateTime.Now.ToString()
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateNewsObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task CannotCreateNewsObjectWithoutPartition()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      NewsObject newsObject = new NewsObject()
      {
        Id = Guid.NewGuid().ToString(),
        Doctype = DocTypes.Image,
        Name = "Example Image",
        Url = "http://somenewswebsite.co.uk/news",
        DatePublished = DateTime.Now.ToString()
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateNewsObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task CannotCreateNewsObjectWithoutDoctype()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      NewsObject newsObject = new NewsObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Name = "Example Image",
        Url = "http://somenewswebsite.co.uk/news",
        DatePublished = DateTime.Now.ToString()
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateNewsObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task CannotCreateNewsObjectWithoutName()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      NewsObject newsObject = new NewsObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Doctype = DocTypes.Image,
        Url = "http://somenewswebsite.co.uk/news",
        DatePublished = DateTime.Now.ToString()
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateNewsObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task CannotCreateNewsObjectWithoutUrl()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      NewsObject newsObject = new NewsObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = "Example Image",
        DatePublished = DateTime.Now.ToString()
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateNewsObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task CannotCreateNewsObjectWithoutDatePublished()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      NewsObject newsObject = new NewsObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = "Example Image",
        Url = "http://somenewswebsite.co.uk/news"
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateNewsObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task CanCreateImagesFromSearch()
    {
      String showId = "5";
      int count = 10;
      ILogger log = new StubLogger();
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        Doctype = DocTypes.Image,
        ShowName = "Phantom of the Opera"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateImageObjectsFromSearch(message, log, count));

      // Assert
      Assert.Null(ex);
      Assert.Equal(count, (await _showDomain.GetImageByShow(showId)).Count);
    }


    [Fact]
    public async Task CanCreateNewsFromSearch()
    {
      String showId = "5";
      int count = 10;
      ILogger log = new StubLogger();
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        Doctype = DocTypes.Image,
        ShowName = "Phantom of the Opera"
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateNewsObjectsFromSearch(message, log, count));

      // Assert
      Assert.Null(ex);
      Assert.Equal(count, (await _showDomain.GetNewsByShow(showId)).Count);
    }


    [Fact]
    public async Task CannotCreateImageWithoutName()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      ImageObject imageObject = new ImageObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Doctype = DocTypes.Image,
        ContentUrl = "http://localhost/something.jpg"
      };


      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateImageObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }


    [Fact]
    public async Task CannotCreateImageWithoutContentUrl()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      ImageObject imageObject = new ImageObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = "Example Image"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateImageObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }

    [Fact]
    public async Task CannotCreateImageWithoutId()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      ImageObject imageObject = new ImageObject()
      {
        Partition = showId,
        Doctype = DocTypes.Image,
        Name = "Example Image",
        ContentUrl = "http://localhost/something.jpg"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateImageObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }


    [Fact]
    public async Task CannotCreateImageWithoutPartition()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      ImageObject imageObject = new ImageObject()
      {
        Id = Guid.NewGuid().ToString(),
        Doctype = DocTypes.Image,
        Name = "Example Image",
        ContentUrl = "http://localhost/something.jpg"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateImageObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }


    [Fact]
    public async Task CannotCreateImageWithoutDoctype()
    {
      String showId = "5";
      ILogger log = new StubLogger();
      ImageObject imageObject = new ImageObject()
      {
        Id = Guid.NewGuid().ToString(),
        Partition = showId,
        Name = "Example Image",
        ContentUrl = "http://localhost/something.jpg"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.CreateImageObject(message, log));

      // Assert
      Assert.NotNull(ex);
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }

    [Fact]
    public async Task UpdateShowSucceedsWithValidData()
    {
      String showId = "1";
      String showName = $"Updated Show Object for Show #{showId}";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        Doctype = DocTypes.Show,
        ShowName = showName
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      bool updatedSuccessfully = await _showDomain.UpdateShowObject(message);

      // Assert
      Assert.True(updatedSuccessfully);
      Assert.Equal(showName, (await _showDomain.GetShow(showId)).ShowName);
    }

    [Fact]
    public async Task UpdateShowThrowsWithNonExistingIdAndNonExistingPartition()
    {
      string showId = "5";
      string showName = $"Updated Show Object for Show #{showId}";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = showId,
        Doctype = DocTypes.Show,
        ShowName = showName
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateShowObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }

    [Fact]
    public async Task UpdateShowThrowsWithNonExistingIdAndExistingPartition()
    {
      string showId = "5";
      string randomPartition = "1";
      string showName = $"Updated Show Object for Show #{showId}";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Partition = randomPartition,
        Doctype = DocTypes.Show,
        ShowName = showName
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateShowObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }


    [Fact]
    public async Task UpdateShowThrowsWithMissingId()
    {
      string showId = "5";
      string randomPartition = "1";
      string showName = $"Updated Show Object for Show #{showId}";
      ShowObject showObject = new ShowObject()
      {
        Partition = randomPartition,
        Doctype = DocTypes.Show,
        ShowName = showName
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateShowObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }


    [Fact]
    public async Task UpdateShowThrowsWithMissingPartition()
    {
      string showId = "5";
      string showName = $"Updated Show Object for Show #{showId}";
      ShowObject showObject = new ShowObject()
      {
        Id = showId,
        Doctype = DocTypes.Show,
        ShowName = showName
      };

      MessageObject<ShowObject> message = new MessageObject<ShowObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = showObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateShowObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.Null(await _showDomain.GetShow(showId));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task UpdateImageSucceedsWithValidData(string showId)
    {
      int idAsInteger = 0;
      Int32.TryParse(showId, out idAsInteger);
      int numberOfChildObjects = idAsInteger * idAsInteger;

      // Act
      for (int i = 0; i < numberOfChildObjects; i++)
      {
        String imageName = $"Updated Show Object for Image #{i}";
        ImageObject imageObject = new ImageObject()
        {
          Id = $"i{i}",
          Partition = showId,
          Doctype = DocTypes.Image,
          ContentUrl = "http://localhost.com/someimage.jpg",
          Name = "Blah"
        };

        MessageObject<ImageObject> message = new MessageObject<ImageObject>
        {
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          },
          Body = imageObject
        };
        bool updatedSuccessfully = await _showDomain.UpdateImageObject(message);
        Assert.True(updatedSuccessfully);
      }

      Assert.Equal(numberOfChildObjects, (await _showDomain.GetImageByShow(showId)).Count);
      Assert.NotNull(await _showDomain.GetShow(showId));
    }

    [Fact]
    public async Task UpdateImageThrowsWithNonExistingIdAndNonExistingPartition()
    {
      string showId = "5";
      String imageName = $"Updated Image Name for Image #{showId}";
      ImageObject imageObject = new ImageObject()
      {
        Id = $"i{showId}",
        Partition = showId,
        Doctype = DocTypes.Image,
        ContentUrl = "http://localhost.com/someimage.jpg",
        Name = "Blah"
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateImageObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.NotNull(await _showDomain.GetImageByShow(showId));
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }

    [Fact]
    public async Task UpdateImageThrowsWithNonExistingIdAndExistingPartition()
    {
      string showId = "5";
      string randomPartition = "1";
      string name = $"Updated Show Object for Show #{showId}";
      ImageObject imageObject = new ImageObject()
      {
        Id = $"i{showId}",
        Partition = randomPartition,
        Doctype = DocTypes.Image,
        ContentUrl = "http://localhost.com/someimage.jpg",
        Name = name
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateImageObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.NotNull(await _showDomain.GetImageByShow(showId));
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }


    [Fact]
    public async Task UpdateImageThrowsWithMissingId()
    {
      string showId = "5";
      string name = $"Updated Show Object for Show #{showId}";

      ImageObject imageObject = new ImageObject()
      {
        Id = $"i{showId}",
        Doctype = DocTypes.Image,
        ContentUrl = "http://localhost.com/someimage.jpg",
        Name = name
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateImageObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.NotNull(await _showDomain.GetImageByShow(showId));
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }


    [Fact]
    public async Task UpdateImageThrowsWithMissingPartition()
    {
      string showId = "5";
      string name = $"Updated Show Object for Show #{showId}";
      ImageObject imageObject = new ImageObject()
      {
        Id = $"i{showId}",
        Partition = showId,
        Doctype = DocTypes.Image,
        ContentUrl = "http://localhost.com/someimage.jpg",
        Name = name
      };

      MessageObject<ImageObject> message = new MessageObject<ImageObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = imageObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateImageObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.NotNull(await _showDomain.GetImageByShow(showId));
      Assert.Equal(0, (await _showDomain.GetImageByShow(showId)).Count);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    public async Task UpdateNewsSucceedsWithValidData(string showId)
    {
      int idAsInteger = 0;
      Int32.TryParse(showId, out idAsInteger);
      int numberOfChildObjects = idAsInteger * idAsInteger;

      // Act
      for (int i = 0; i < numberOfChildObjects; i++)
      {
        String newsName = $"Updated Show Object for News #{i}";
        NewsObject newsObject = new NewsObject()
        {
          Id = $"n{i}",
          Partition = showId,
          Doctype = DocTypes.News,
          DatePublished = DateTime.Now.ToString(),
          Url = "http://localhost.com/someimage.jpg",
          Name = newsName
        };

        MessageObject<NewsObject> message = new MessageObject<NewsObject>
        {
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          },
          Body = newsObject
        };
        bool updatedSuccessfully = await _showDomain.UpdateNewsObject(message);
        Assert.True(updatedSuccessfully);
      }

      Assert.Equal(numberOfChildObjects, (await _showDomain.GetNewsByShow(showId)).Count);
      Assert.NotNull(await _showDomain.GetShow(showId));
    }

    [Fact]
    public async Task UpdateNewsThrowsWithNonExistingIdAndNonExistingPartition()
    {
      string showId = "5";
      String newsName = $"Updated News Name for News #{showId}";

      NewsObject newsObject = new NewsObject()
      {
        Id = $"n{showId}",
        Partition = showId,
        Doctype = DocTypes.News,
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost.com/someimage.jpg",
        Name = newsName
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateNewsObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.NotNull(await _showDomain.GetNewsByShow(showId));
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }

    [Fact]
    public async Task UpdateNewsThrowsWithNonExistingIdAndExistingPartition()
    {
      string showId = "5";
      string randomPartition = "1";
      string name = $"Updated News Name for News #n{showId}";

      String imageName = $"Updated Show Object for News #{showId}";
      NewsObject newsObject = new NewsObject()
      {
        Id = $"n{showId}",
        Partition = randomPartition,
        Doctype = DocTypes.News,
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost.com/someimage.jpg",
        Name = "Blah"
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateNewsObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.NotNull(await _showDomain.GetNewsByShow(showId));
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }


    [Fact]
    public async Task UpdateNewsThrowsWithMissingId()
    {
      string showId = "5";
      string name = $"Updated News Object for Show #{showId}";
      NewsObject newsObject = new NewsObject()
      {
        Partition = showId,
        Doctype = DocTypes.News,
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost.com/someimage.jpg",
        Name = name
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateNewsObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.NotNull(await _showDomain.GetNewsByShow(showId));
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }


    [Fact]
    public async Task UpdateNewsThrowsWithMissingPartition()
    {
      string showId = "5";
      string name = $"Updated News Object for Show #{showId}";
      NewsObject newsObject = new NewsObject()
      {
        Id = $"n{showId}",
        Doctype = DocTypes.News,
        DatePublished = DateTime.Now.ToString(),
        Url = "http://localhost.com/someimage.jpg",
        Name = name
      };

      MessageObject<NewsObject> message = new MessageObject<NewsObject>
      {
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = Guid.NewGuid().ToString(),
          RequestCreatedAt = DateTime.Now
        },
        Body = newsObject
      };

      // Act
      var ex = await Record.ExceptionAsync(() => _showDomain.UpdateNewsObject(message));

      // Assert
      Assert.NotNull(ex);
      Assert.NotNull(await _showDomain.GetNewsByShow(showId));
      Assert.Equal(0, (await _showDomain.GetNewsByShow(showId)).Count);
    }
  }
}
