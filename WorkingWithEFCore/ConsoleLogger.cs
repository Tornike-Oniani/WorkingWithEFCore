using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingWithEFCore
{
  internal class ConsoleLoggerProvider : ILoggerProvider
  {
    public ILogger CreateLogger(string categoryName)
    {
      // we could have different Logger implementations for
      // different categoryName values but we only have one
      return new ConsoleLogger();
    }

    // if your logger uses unmanaged resources,
    // then you can release them here
    public void Dispose() { }
  }

  internal class ConsoleLogger : ILogger
  {
    // if your logger uses unmanaged resources, you can
    // return the class that implements IDisposable here
    public IDisposable BeginScope<TState>(TState state)
    {
      return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      // to avoid overloggin, you can filter on the log leve
      switch (logLevel)
      {
        case LogLevel.Trace:
        case LogLevel.Information:
        case LogLevel.None:
          return false;
        case LogLevel.Debug:
        case LogLevel.Warning:
        case LogLevel.Error:
        case LogLevel.Critical:
        default:
          return true;
      }
    }

    public void Log<TState>
      (
        LogLevel logLevel, 
        EventId eventId, 
        TState state, 
        Exception? exception, 
        Func<TState, Exception?, string> formatter
      )
    {
      // log only events with id 20100
      // (this id is for printing out LINQ query to SQL statemenst)
      if (eventId.Id != 20100) { return; }

      // log the level and event identifier
      Console.WriteLine($"Level: {logLevel}, Event Id: {eventId.Id}");

      // only output the state or exception if it exists
      if (state != null)
      {
        Console.Write($", State: {state}"); ;
      }

      if (exception != null)
      {
        Console.Write($", Exception: {exception.Message}");
      }

      Console.WriteLine();
    }
  }
}
