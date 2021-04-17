using System;

using NArgs.Attributes;

namespace NArgsTest.CommandLineParserTests.Data
{
  public class TestHelpOptionConfiguration
  {
    [Option(Name = "R1", Required = true)]
    public bool R1
    {
      get;
      set;
    }

    [Command(Name = "c1", Description = "Example command #1")]
    public TestCommand Command1
    {
      get;
      set;
    } = new TestCommand();

    [HelpOption(Name = "H", AlternativeName = "?", LongName = "help")]
    public bool ShowUsage
    {
      get;
      set;
    }
  }
}
