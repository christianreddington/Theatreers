using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theatreers.Core.Abstractions;
using Theatreers.Core.Models;
using Theatreers.Core.Providers;
using Xunit;

namespace Theatreers.Core.Tests
{
  public class StorageProviderTest : IDisposable
  {
    private IStorageProvider<CosmosBaseObject<string>> _storageProvider;
    public StorageProviderTest()
    {
      ILogger log = new StubLogger();

      IDocumentClient client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
      Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
      _storageProvider = new CosmosStorageProvider<CosmosBaseObject<string>>(client, showCollectionUri);
      // _storageProvider = new LocalMemoryProvider<CosmosBaseObject<string>>();
      _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "1",
        Partition = "blah",
        InnerObject = "hello",
        Doctype = "show"
      }, log);
      _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "2",
        Partition = "blah",
        InnerObject = "hello",
        Doctype = "show"
      }, log);
    }

    public void Dispose()
    {
      ILogger log = new StubLogger();
      //_storageProvider = null;
      _storageProvider.DeleteAsync("1", log);
      _storageProvider.DeleteAsync("2", log);
    }

    [Fact]
    public async void CanCreateAsyncWithValidData()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
       await _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "3",
        Partition = "blah",
        InnerObject = "hello",
        Doctype = "show"
      }, log);


      // Assert  
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(3, query.Select(e => e.Id).ToList<String>().Count);
    }

    [Fact]
    public async void CannotCreateAsyncForValuesWithExistingKey()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      Exception _insert = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "2",
        Partition = "blah",
        InnerObject = "hello",
        Doctype = "show"
      }, log));


      // Assert  
      Assert.IsAssignableFrom<ArgumentException>(_insert);
    }


    [Fact]
    public async void CannotCreateAsyncForValuesWithMissingPartition()
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
      Assert.Equal(2, query.Count());
    }

    [Fact]
    public async void CannotCreateAsyncForValuesWithMissingId()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Partition = "blah",
        Doctype = "show",
        InnerObject = "hello"
      }, log));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Count());
    }


    [Fact]
    public async void CannotCreateAsyncForValuesWithMissingDoctype()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "3",
        Partition = "blah",
        InnerObject = "hello"
      }, log));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Count());
    }


    [Fact]
    public async void CannotCreateAsyncForValuesWithMissingInnerObject()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new CosmosBaseObject<string>
      {
        Id = "3",
        Doctype = "show",
        Partition = "blah"
      }, log));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.Equal(2, query.Count());
    }

    [Fact]
    public async void CreateAsyncWithNullObjectThrowsArgumentNullException()
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
    public async void CheckQueryReturnsAsIQueryable()
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
    public async void CheckExistsAsyncReturnsTrueWithValidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();

      // Act  
      bool objectExists = await _storageProvider.CheckExistsAsync(reference, log);

      //Assert
      Assert.True(objectExists);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    public async void CheckExistsAsyncReturnsFalseWithInvalidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();

      // Act  
      bool objectExists = await _storageProvider.CheckExistsAsync(reference, log);

      //Assert
      Assert.False(objectExists);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void DeleteAsyncSucceedsWithValidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();

      // Act  
      var deletion = await _storageProvider.DeleteAsync(reference, log);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.True(deletion); 
      Assert.Single(query);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async void DeleteAsyncFailsWithInvalidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();

      // Act 
      var deletion = await _storageProvider.DeleteAsync(reference, log);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.False(deletion);
      Assert.Equal(2, query.Count());
    }


    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void UpdateAsyncSucceedsWithValidKeyAndData(string reference)
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
      Assert.Equal(2, query.Count());
      Assert.Equal("myNewString", query.Where(e => e.Id == reference).First().InnerObject);
      Assert.Single(query.Where(e => e.InnerObject == "myNewString"));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void UpdateAsyncFailssWithExistingKeyAndMissingInnerObject(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void UpdateAsyncFailssWithExistingKeyAndMissingDoctyype(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void UpdateAsyncFailssWithExistingKeyAndMissingPartition(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async void UpdateAsyncFailsWithInvalidKey(string reference)
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
      var update = await _storageProvider.UpdateAsync(reference, _object, log);

      // Assert
      IQueryable<CosmosBaseObject<string>> query = await _storageProvider.Query();
      Assert.False(update);
      Assert.Equal(2, query.Count());
      Assert.Empty(query.Where(e => e.InnerObject == "myNewString"));
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async void UpsertAsyncCreatesNewObjectWithNewKeyAndValidData(string reference)
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
      Assert.Equal(3, query.Count());
      Assert.Single(query.Where(e => e.InnerObject == "myNewString"));
      Assert.Equal(reference, query.Where(e => e.InnerObject == "myNewString").SingleOrDefault().Id);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async void UpsertAsyncCannotCompleteWithMissingId(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async void UpsertAsyncCannotCreateNewObjectWithNewKeyAndMissingPartition(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async void UpsertAsyncCannotCreateNewObjectWithNewKeyAndMissingInnerObject(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async void UpsertAsyncCannotCreateNewObjectWithNewKeyAndMissingDoctype(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void UpsertAsyncUpdatesObjectWithExistingKey(string reference)
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
      Assert.Equal(2, query.Count());
      Assert.Single(query.Where(e => e.InnerObject == "myNewString"));
      Assert.Equal(reference, query.Where(e => e.InnerObject == "myNewString").SingleOrDefault().Id);
    }


    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void UpsertAsyncFailsToUpdateObjectWithExistingKeyAndMissingPartition(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void UpsertAsyncFailsToUpdateObjectWithExistingKeyAndMissingInnerObject(string reference)
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
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async void UpsertAsyncFailsToUpdateObjectWithExistingKeyAndMissingDoctype(string reference)
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
      Assert.Equal(2, query.Count());
    }
  }
}
