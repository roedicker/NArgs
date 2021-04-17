using System;

using NArgs.Attributes;

namespace NArgsTest.CommandLineParserTests.Data
{
  public class TestCommandConfiguration
  {
    [Option(Name = "s1", LongName ="switch-1")]
    public bool Switch1
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
  }
}
