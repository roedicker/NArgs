using System;

using NArgs;
using NArgs.Attributes;

namespace NArgsTest.CommandLineParserTests.Data
{
  public class TestCommand : ICommand
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
