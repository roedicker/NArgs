using System;

using NArgs;
using NArgs.Attributes;

namespace NArgsTest.CommandLineParserTests.Data
{
  public class ValidCommandConfiguration
  {
    [Option(Name = "s1", LongName ="switch-1")]
    public bool Switch1
    {
      get;
      set;
    }

    [Command(Name = "c1", Description = "Example command #1")]
    public ExampleCommandConfiguration Command1
    {
      get;
      set;
    } = new ExampleCommandConfiguration();
  }

  public class ExampleCommandConfiguration : ICommand
  {
    [Option(Name = "s2", LongName = "switch-2")]
    public bool Switch2
    {
      get;
      set;
    }

    [Parameter(Name = "p1", OrdinalNumber = 1)]
    public string Name1
    {
      get;
      set;
    }

    /// <summary>
    /// Gets an indicator whether the command has been executed via interface or not.
    /// </summary>
    public bool HasCommandBeenExecuted
    {
      get;
      private set;
    }

    /// <inheritdoc />
    public void ExecuteCommand()
    {
      HasCommandBeenExecuted = true;
    }
  }
}
