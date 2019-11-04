using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theatreers.Core.Providers;

namespace Theatreers.Core.Abstractions
{

  public interface IStorageProvider<T> where T : IStorable, IValidatable
  {
    Task<IQueryable<T>> Query();
    Task<T> ReadAsync(T _object);
    Task CreateAsync(T _object);
    Task<bool> UpdateAsync(T _object);
    Task UpsertAsync(T _object);
    Task<bool> DeleteAsync(T _object);
    Task<bool> CheckExistsAsync(T _object);
  }
}
