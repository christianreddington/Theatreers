using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theatreers.Core.Providers;

namespace Theatreers.Core.Abstractions
{
  public interface IStorageProvider<T>
  {
    Task<IQueryable<KeyValuePair<string, T>>> Query();
    Task<KeyValuePair<string, T>> ReadAsync(string reference);
    Task CreateAsync(T _object, ILogger log);
    Task<bool> UpdateAsync(string reference, T _object, ILogger log);
    Task UpsertAsync(string reference, T _object, ILogger log);
    Task<bool> DeleteAsync(string reference, ILogger log);
    Task<bool> CheckExistsAsync(string reference, ILogger log);
  }
}
