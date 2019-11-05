using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Theatreers.Core.Models
{
  public class StubLogger : ILogger, IDisposable
  {
    public IDisposable BeginScope<TState>(TState state)
    {
      return this;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
    }

    protected virtual void Dispose(bool disposing)
    {

    }

    public void Dispose()
    {
      Dispose(true);
    }
  }
}
