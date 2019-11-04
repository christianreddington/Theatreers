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
  public class NonPartitionedStorageProviderTest : IDisposable
  {
    private IStorageProvider<TestObject> _storageProvider;
    public NonPartitionedStorageProviderTest()
    {

      _storageProvider = new LocalMemoryProvider<TestObject>();
      _storageProvider.CreateAsync(new TestObject
      {
        Id = "1",

        InnerObject = "hello"
      });
      _storageProvider.CreateAsync(new TestObject
      {
        Id = "2",

        InnerObject = "hello"
      });
    }

    public void Dispose()
    {

      IEnumerable<TestObject> documents = new List<TestObject>()
      {
        new TestObject { Id = "1"},
        new TestObject { Id = "2"},
        new TestObject { Id = "-123819"},
        new TestObject { Id = "10"},
        new TestObject { Id = "abc"},
        new TestObject { Id = "3"}
      };

      foreach (TestObject document in documents)
      {
        _storageProvider.DeleteAsync(document);
      }
    }

    [Fact]
    public async Task CanCreateAsyncWithValidData()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      await _storageProvider.CreateAsync(new TestObject
      {
        Id = "3",
        InnerObject = "hello"
      });


      // Assert  
      IQueryable<TestObject> query = await _storageProvider.Query();
      Assert.Equal(3, query.Select(e => e.Id).ToList<String>().Count);
    }

    [Fact]
    public async Task CannotCreateAsyncForValuesWithExistingKey()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      Exception _insert = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new TestObject
      {
        Id = "2",
        InnerObject = "hello"
      }));


      // Assert  
      Assert.IsAssignableFrom<Exception>(_insert);
    }

    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingId()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new TestObject
      {
        InnerObject = "hello"
      }));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<TestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Count());
    }

    [Fact]
    public async Task CannotCreateAsyncForValuesWithMissingInnerObject()
    {
      // Arrange
      ILogger log = new StubLogger();

      // Act
      var exception = await Record.ExceptionAsync(() => _storageProvider.CreateAsync(new TestObject()
      {
        Id = "3"
      }));


      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<TestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Count());
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
      TestObject _object = new TestObject { Id = reference };

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
      TestObject _object = new TestObject { Id = reference };

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
      TestObject _object = new TestObject { Id = reference };

      // Act  
      var deletion = await _storageProvider.DeleteAsync(_object);
      Thread.Sleep(2000);

      // Assert
      IQueryable<TestObject> query = await _storageProvider.Query();
      Assert.True(deletion);
      Assert.Single(query);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("abc")]
    [InlineData("-123819")]
    public async Task DeleteAsyncFailsWithInvalidData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      TestObject _object = new TestObject { Id = reference };

      // Act 
      var deletion = await _storageProvider.DeleteAsync(_object);
      Thread.Sleep(2000);

      // Assert
      IQueryable<TestObject> query = await _storageProvider.Query();
      Assert.False(deletion);
      Assert.Equal(2, query.Count());
    }


    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncSucceedsWithValidKeyAndData(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      TestObject _object = new TestObject()
      {
        Id = reference,
        InnerObject = "myNewString"
      };

      // Act  
      var update = await _storageProvider.UpdateAsync(_object);

      // Assert
      IQueryable<TestObject> query = (IQueryable<TestObject>)await _storageProvider.Query();
      Assert.True(update);
      Assert.Equal(2, query.Count());
      Assert.Equal("myNewString", query.Where(e => e.Id == reference).Take(1).ToList().First().InnerObject);
      Assert.Single(query.Where(e => e.InnerObject == "myNewString"));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpdateAsyncFailsWithExistingKeyAndMissingInnerObject(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      TestObject _object = new TestObject()
      {
        Id = reference
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<TestObject> query = (IQueryable<TestObject>)await _storageProvider.Query();
      Assert.Equal(2, query.Count());
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
      TestObject _object = new TestObject()
      {
        Id = reference,
        InnerObject = "myNewString"

      };
      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpdateAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<TestObject> query = await _storageProvider.Query() as IQueryable<TestObject>;
      Assert.Equal(2, query.Count());
      Assert.Empty(query.Where(e => e.InnerObject == "myNewString"));
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
      TestObject _object = new TestObject()
      {
        Id = reference,
        InnerObject = "myNewString"
      };

      // Act  
      await _storageProvider.UpsertAsync(_object);

      // Assert
      IQueryable<TestObject> query = await _storageProvider.Query() as IQueryable<TestObject>;
      Assert.Equal(3, query.Count());
      Assert.Single(query.Where(e => e.InnerObject == "myNewString"));
      Assert.Equal(reference, query.Where(e => e.InnerObject == "myNewString").Take(1).ToList().First().Id);
    }

    [Fact]
    public async Task UpsertAsyncCannotCompleteWithMissingId()
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      TestObject _object = new TestObject()
      {
        InnerObject = "myNewString"
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<TestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Count());
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
      TestObject _object = new TestObject()
      {
        Id = reference
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<TestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Count());
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpsertAsyncUpdatesObjectWithExistingKey(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      TestObject _object = new TestObject()
      {
        Id = reference,
        InnerObject = "myNewString"
      };

      // Act  
      await _storageProvider.UpsertAsync(_object);

      // Assert
      IQueryable<TestObject> query = (IQueryable<TestObject>)await _storageProvider.Query();
      Assert.Equal(2, query.Count());
      Assert.Single(query.Where(e => e.InnerObject == "myNewString"));
      Assert.Equal(reference, query.Where(e => e.InnerObject == "myNewString").Take(1).ToList().First().Id);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task UpsertAsyncFailsToUpdateObjectWithExistingKeyAndMissingInnerObject(string reference)
    {
      // Arrange
      // Use existing records from constructor
      ILogger log = new StubLogger();
      TestObject _object = new TestObject()
      {
        Id = reference
      };

      // Act  
      var exception = await Record.ExceptionAsync(() => _storageProvider.UpsertAsync(_object));

      // Assert  
      Assert.IsType<Exception>(exception);
      Assert.Equal("There was at least one validation error. Please provide the appropriate information.", exception.Message);
      IQueryable<TestObject> query = await _storageProvider.Query();
      Assert.Equal(2, query.Count());
    }
  }
}