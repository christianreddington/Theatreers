using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Theatreers.Show.Test
{
  public class UnitTest1 : IDisposable
  {
    dynamic documentClient;

    public UnitTest1(){
      documentClient = new DocumentClient(new System.Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");      
    }

    public void Dispose()
    {
      // ... clean up test data from the database ...
    }


    [Fact]
    public async void GetObjectsReturnsNoValues()
    {
      // Arrange
      // Mock<HttpRequest> req = new Mock<HttpRequest>();
      Mock<ILogger> log = new Mock<ILogger>();
      // req.SetupProperty(c => c.HttpContext.Request.Path, new PathString("/my/path/is/here"));

      // Act
      var result = await Theatreers.Show.Utils.ShowServiceHelper.GetObjectsAsync("show", "/api/show/notarealguid/show", documentClient, log.Object, "shows", "show");

      // Assert
      Assert.IsAssignableFrom<Microsoft.AspNetCore.Mvc.StatusCodeResult>(result);
      var objectResponse = result as Microsoft.AspNetCore.Mvc.StatusCodeResult;
      Assert.Equal(404 , objectResponse.StatusCode);
    }
  }
}
