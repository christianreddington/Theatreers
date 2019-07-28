using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Theatreers.Core.Models;
using Theatreers.Show.Models;

namespace Theatreers.Show.Abstractions
{
  public interface IShowDomain
  {
    Task<ShowObject> GetShow(string id);
    Task<ICollection<ShowListObject>> GetShowList(string letter);
    Task CreateImageObjectsFromSearch(MessageObject<ShowObject> message, ILogger log);
  }
}