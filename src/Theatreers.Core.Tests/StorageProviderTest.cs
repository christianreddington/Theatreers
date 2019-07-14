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
  public class StorageProviderTest  : IDisposable
  {
    private IStorageProvider<CosmosBaseObject<string>> _storageProvider;
    public StorageProviderTest()
    {
      ILogger log = new StubLogger();

      string databaseName = "theatreers";
      string collectionName = "shows";

      IDocumentClient client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
      Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
      _storageProvider = new CosmosStorageProvider<CosmosBaseObject<string>>(client, showCollectionUri, databaseName, collectionName);
      // _storageProvider = new LocalMemoryProvider<CosmosBaseObject<string>>();
    _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "1",
        Partition = "partition",
        InnerObject = "hello",
        Doctype = "show"
      }, log);
      _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "2",
        Partition = "partition",
        InnerObject = "hello",
        Doctype = "show"
      }, log);
    }

    public void Dispose()
    {
      ILogger log = new StubLogger();
      // _storageProvider = null;
      IDocumentClient client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

      IEnumerable<string> documents = new List<string>()
      {
        "1",
        "2",
        "-123819",
        "10",
        "abc",
        "3"
      };

      foreach (string documentId in documents)
      {
        _storageProvider.DeleteAsync(documentId, "partition", log);
      }
    }

    [Fact]
    public async Task CanCreateAsyncWithValidData()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
       await _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "3",
        Partition = "partition",
        InnerObject = "hello",
        Doctype = "show"
      }, log);


      // Assert  
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(3, query.Where(doc => doc.Partition == "partition").Select(e => e.Id).ToList<String>().Count);
    }

    [Fact]
    public async Task CannotCreateAsyncForValuesWithExistingKey()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      Exception _insert = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "2",
        Partition = "partition",
        InnerObject = "hello",
        Doctype = "show"
      }, log));


      // Assert  
      Assert.IsAssignableFrom<ArgumentException>(_insert);
    }


    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingPartition()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "3",
        Doctype = "show",
        InnerObject = "hello"
      }, log));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(0, query.Where(doc => doc.Partition == "").Count());
    }

    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingId()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Partition = "partition",
        Doctype = "show",
        InnerObject = "hello"
      }, log));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }


    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingDoctype()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "3",
        Partition = "partition",
        InnerObject = "hello"
      }, log));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }


    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingInnerObject()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "3",
        Doctype = "show",
        Partition = "partition"
      }, log));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Fact]
    public async Task CreateAsyncWithNullObjectThrowsArgumentNullException()
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();

      // Act
      Exception exception = await Record.ExceptionAsync(() => 
                              _storageProvider.CreateAsync(null, log)
                            );

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
      ILogger log = new StubLogger();

      // Act  
      bool objectExists = await _storageProvider.CheckExistsAsync(reference, "partition", log);

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
      ILogger log = new StubLogger();

      // Act  
      bool objectExists = await _storageProvider.CheckExistsAsync(reference, "partition", log);

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
      ILogger log = new StubLogger();

      // Act  
      var deletion = await _storageProvider.DeleteAsync(reference, "partition", log);
      Thread.Sleep(2000);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      ILogger log = new StubLogger();

      // Act 
      var deletion = await _storageProvider.DeleteAsync(reference, "partition", log);
      Thread.Sleep(2000);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var update = await _storageProvider.UpdateAsync(reference, _object, log);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.True(update);
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
      Assert.Equal("myNewString", query.Where(e => e.Id == reference && e.Partition == "partition").Take(1).ToList().First().InnerObject);
      Assert.Single(query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition"));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncFailssWithExistingKeyAndMissingInnerObject(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Partition = "partition",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncFailssWithExistingKeyAndMissingDoctype(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Partition = "partition",
        InnerObject = "myNewString"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncFailssWithExistingKeyAndMissingPartition(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
        
      };

      // Act  
      var update = await _storageProvider.UpdateAsync(reference, _object, log);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.False(update);
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      await _storageProvider.UpsertAsync(reference, _object, log);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(3, query.Where(e => e.Partition == "partition").Count());
      Assert.Single(query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition"));
      Assert.Equal(reference, query.Where(e => e.InnerObject == "myNewString" && e.Partition == "partition").Take(1).ToList().First().Id);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async Task UpsertAsyncCannotCompleteWithMissingId(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Id = reference,
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Id = reference,
        Partition = "partition",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Id = reference,
        Partition = "partition",
        InnerObject = "myNewString"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Partition = "partition",
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      await _storageProvider.UpsertAsync(reference, _object, log);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        InnerObject = "myNewString",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Partition = "partition",
        Doctype = "show"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
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
      CosmosBaseObject<string> _object = new CosmosBaseObject<string>()
      {
        Partition = "partition",
        InnerObject = "myNewString"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(reference, _object, log));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Where(doc => doc.Partition == "partition").Count());
    }
  }
}
