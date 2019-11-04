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
  public class LocalMemoryProvider<T> : IStorageProvider<T> where T : IStorableValidatable
  {
    private IDictionary<string, T> _datastore = new ConcurrentDictionary<string, T>();
    public async Task<bool> CheckExistsAsync(T _object)
    {
      return _datastore.Where(e => e.Key == _object.Id).Any();
    }

    public async Task CreateAsync(T value)
    {
      if (value.IsValid())
      {
        dynamic key = value.Id;
        _datastore.Add(key, value);
      }
      else
      {
        throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
      }
    }

    public async Task<bool> DeleteAsync(T _object)
    {
      return _datastore.Remove(_object.Id);
    }

    public async Task<IQueryable<T>> Query()
    {
      return _datastore.Select(e => e.Value).AsQueryable();
    }

    public async Task<T> ReadAsync(T _object)
    {
      return this.Query().Result.Where(e => e.Id == _object.Id).SingleOrDefault();
    }

    public async Task<bool> UpdateAsync(T _object)
    {
      if (_object != null && await CheckExistsAsync(_object))
      {
        if (_object.IsValid())
        {
          _datastore[_object.Id] = _object;
          return true;
        } else
        {
          throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
        }
      } else {
        throw new Exception($"There was at least one validation error. Please provide the appropriate information.");
      }
    }

    public async Task UpsertAsync(T _object)
    {
      T result = this.Query().Result.Where(e => e.Id == _object.Id).SingleOrDefault();

      if (result != null)
      {
        await UpdateAsync(_object);
      }
      else {
        await CreateAsync(_object);
      }
    }
  }
}
