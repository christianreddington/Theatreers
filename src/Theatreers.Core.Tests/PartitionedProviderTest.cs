using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theatreers.Core.Abstractions;
using Theatreers.Core.Models;
using Theatreers.Core.Providers;
using Xunit;

namespace Theatreers.Core.Tests
{
  public class StorageProviderTest  : IAsyncLifetime
  {
    private IStorageProvider<PartitionedTestObject> _storageProvider;
    public async Task InitializeAsync()
    {

      string databaseName = "theatreers";
      string collectionName = "shows";

      IDocumentClient client = new DocumentClient(new Uri(getCosmosURI()), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
      Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
      Uri databaseUri = UriFactory.CreateDatabaseUri(databaseName);

      Database theatreersDatabase = new Database()
      {
        Id = databaseName
      };

      await client.CreateDatabaseIfNotExistsAsync(theatreersDatabase);

      DocumentCollection showCollection = new DocumentCollection()
      {
        Id = collectionName
      };

      showCollection.PartitionKey.Paths.Add("/partition");
      showCollection.DefaultTimeToLive = -1;
      await client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, showCollection);
 
      _storageProvider = new CosmosStorageProvider<PartitionedTestObject>(client, showCollectionUri, databaseName, collectionName);
      await _storageProvider.CreateAsync(new PartitionedTestObject
      {
        Id = "1",
        Partition = "partition",
        InnerObject = "hello",
        Doctype = "show"
      });
      await _storageProvider.CreateAsync(new PartitionedTestObject
      {
        Id = "2",
        Partition = "partition",
        InnerObject = "hello",
        Doctype = "show"
      });
    }

    public string getCosmosURI(){
      if (System.Environment.GetEnvironmentVariable("AZURE_COSMOS_DB_CONNECTION_STRING") == null){
        return "https://localhost:8081";
      }

      return System.Environment.GetEnvironmentVariable("AZURE_COSMOS_DB_CONNECTION_STRING");
    }

    public async Task DisposeAsync()
    {
       IDocumentClient client = new DocumentClient(new Uri(getCosmosURI()), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

      IEnumerable<PartitionedTestObject> documents = new List<PartitionedTestObject>()
      {
        new PartitionedTestObject { Id = "1", Partition = "partition" },
        new PartitionedTestObject { Id = "2", Partition = "partition" },
        new PartitionedTestObject { Id = "-123819", Partition = "partition" },
        new PartitionedTestObject { Id = "10", Partition = "partition" },
        new PartitionedTestObject { Id = "abc", Partition = "partition" },
        new PartitionedTestObject { Id = "3", Partition = "partition" }
      };

      foreach (PartitionedTestObject document in documents)
      {
        await _storageProvider.DeleteAsync(document);
      }
          await  Task.Delay(6000);
    }

    [Fact]
    public async Task CanCreateAsyncWithValidData()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
       await _storageProvider.CreateAsync(new PartitionedTestObject
      {
        Id = "3",
        Partition = "partition",
        InnerObject = "hello",
        Doctype = "show"
      });


      // Assert  
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(3, query.Where(doc => doc.Partition == "partition").Select(e => e.Id).ToList<String>().Count);
    }

    [Fact]
    public async Task CannotCreateAsyncForValuesWithExistingKey()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      Exception _insert = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new PartitionedTestObject
      {
        Id = "2",
        Partition = "partition",
        InnerObject = "hello",
        Doctype = "show"
      }));


      // Assert  
      Assert.IsAssignableFrom<Exception>(_insert);
    }


    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingPartition()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new PartitionedTestObject
      {
        Id = "3",
        Doctype = "show",
        InnerObject = "hello"
      }));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(0, query.Where(doc => doc.Partition == "").Count());
    }

    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingId()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new PartitionedTestObject
      {
        Partition = "partition",
        Doctype = "show",
        InnerObject = "hello"
      }));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }


    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingDoctype()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new PartitionedTestObject
      {
        Id = "3",
        Partition = "partition",
        InnerObject = "hello"
      }));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }


    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingInnerObject()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new PartitionedTestObject()
      {
        Id = "3",
        Doctype = "show",
        Partition = "partition"
      }));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Fact]
    public async Task CreateAsyncWithNullObjectThrowsArgumentNullException()
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();

      // Act
      Exception exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(null));

      //Assert
      Assert.IsType<System.NullReferenceException>(exception);
     }

    [Fact]
    public async Task CheckQueryReturnsAsIQueryable()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      IQueryable query = await _storageProvider.Query();
      // Assert
      Assert.IsAssignableFrom<IQueryable>(query);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task CheckExistsAsyncReturnsTrueWithValidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      PartitionedTestObject _object = new PartitionedTestObject { Id = reference, Partition = "partition" };

      // Act  
      bool objectExists = await _storageProvider.CheckExistsAsync(_object);

      //Assert
      Assert.True(objectExists);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    public async Task CheckExistsAsyncReturnsFalseWithInvalidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      PartitionedTestObject _object = new PartitionedTestObject { Id = reference, Partition = "partition" };

      // Act  
      bool objectExists = await _storageProvider.CheckExistsAsync(_object);

      //Assert
      Assert.False(objectExists);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task DeleteAsyncSucceedsWithValidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      PartitionedTestObject _object = new PartitionedTestObject { Id = reference, Partition = "partition" };

      // Act  
      var deletion = await _storageProvider.DeleteAsync(_object);
      await Task.Delay(6000);

      // Assert
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.True(deletion);
      Assert.Single(query.Where(e => e.Partition == "partition"));
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async Task DeleteAsyncFailsWithInvalidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      PartitionedTestObject _object = new PartitionedTestObject { Id = reference, Partition = "partition" };

      // Act 
      var deletion = await _storageProvider.DeleteAsync(_object);
      await Task.Delay(6000);

      // Assert
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.False(deletion);
      Assert.Equal(2, query.Where(e => e.Partition == "partition").Count());
    }


    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncSucceedsWithValidKeyAndData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var update = await _storageProvider.UpdateAsync(_object);

      // Assert
      IQueryable<PartitionedTestObject> query = (IQueryable<PartitionedTestObject>) await _storageProvider.Query();
      Assert.True(update);
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
      Assert.Equal("myNewString", query.Where(e => e.Id == reference && e.Partition == "partition").Take(1).ToList().First().InnerObject);
      Assert.Single(query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition"));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncFailsWithExistingKeyAndMissingInnerObject(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = (IQueryable<PartitionedTestObject>)await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncFailsWithExistingKeyAndMissingDoctype(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncFailsWithExistingKeyAndMissingPartition(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async Task UpdateAsyncFailsWithInvalidKey(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
        
      };
      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query() as IQueryable<PartitionedTestObject>;
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
      Assert.Empty(query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition"));
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async Task UpsertAsyncCreatesNewObjectWithNewKeyAndValidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      await _storageProvider.UpsertAsync(_object);

      // Assert
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query() as IQueryable<PartitionedTestObject>;
      Assert.Equal(3, query.Where(e => e.Partition == "partition").Count());
      Assert.Single(query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition"));
      Assert.Equal(reference, query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition").Take(1).ToList().First().Id);
    }

    [Fact]
    public async Task UpsertAsyncCannotCompleteWithMissingId()
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async Task UpsertAsyncCannotCreateNewObjectWithNewKeyAndMissingPartition(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async Task UpsertAsyncCannotCreateNewObjectWithNewKeyAndMissingInnerObject(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async Task UpsertAsyncCannotCreateNewObjectWithNewKeyAndMissingDoctype(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpsertAsyncUpdatesObjectWithExistingKey(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      await _storageProvider.UpsertAsync(_object);

      // Assert
      IQueryable<PartitionedTestObject> query = (IQueryable<PartitionedTestObject>) await _storageProvider.Query();
      Assert.Equal(2, query.Where(e => e.Partition == "partition").Count());
      Assert.Single(query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition"));
      Assert.Equal(reference, query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition").Take(1).ToList().First().Id);
    }


    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpsertAsyncFailsToUpdateObjectWithExistingKeyAndMissingPartition(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpsertAsyncFailsToUpdateObjectWithExistingKeyAndMissingInnerObject(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpsertAsyncFailsToUpdateObjectWithExistingKeyAndMissingDoctype(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      PartitionedTestObject _object = new PartitionedTestObject()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<PartitionedTestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }
  }
}
