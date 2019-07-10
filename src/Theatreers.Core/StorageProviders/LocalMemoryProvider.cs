using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Theatreers.Core.Abstractions;
using Theatreers.Core.Models;

namespace Theatreers.Core.Providers
{
  public class LocalMemoryProvider<T> : IStorageProvider<T> where T : ValidatableBaseObject
  {
    private IDictionary<string, T> _datastore = new ConcurrentDictionary<string, T>();
    public async Task<bool> CheckExistsAsync(string reference, ILogger log)
    {
      return _datastore.Where(e => e.Key == reference).Any();
    }

    public async Task CreateAsync(T value, ILogger log)
    {
        if (value.IsValid())
        {
          dynamic key = value.Id;
          _datastore.Add(key, value);
        } else {
          throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
        }
    }

    public async Task<bool> DeleteAsync(string reference, ILogger log)
    {
      return _datastore.Remove(reference);
    }

    public async Task<IQueryable<KeyValuePair<string, T>>> Query()
    {
      return _datastore.AsQueryable();
    }

    public async Task<KeyValuePair<string, T>> ReadAsync(string reference)
    {
      return this.Query().Result.Where(e => e.Key == reference).SingleOrDefault();
    }

    public async Task<bool> UpdateAsync(string reference, T _object, ILogger log)
    {
      KeyValuePair<string, T> result = this.Query().Result.Where(e => e.Key == reference).SingleOrDefault();

      _object.Id = result.Key;
      if (result.Key != null)
      {
        if (_object.IsValid())
        {
          _datastore[result.Key] = _object;
          return true;
        } else
        {
          throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
        }
      } else {
        return false;
      }
    }

    public async Task UpsertAsync(string reference, T _object, ILogger log)
    {
      KeyValuePair<string, T> result = this.Query().Result.Where(e => e.Key == reference).SingleOrDefault();

      if (result.Key != null)
      {
        await UpdateAsync(reference, _object, log);
      }
      else {
        await CreateAsync(_object, log);
      }
    }
  }
}
