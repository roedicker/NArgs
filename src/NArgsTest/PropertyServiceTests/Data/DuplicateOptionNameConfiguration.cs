using System;

using NArgs.Attributes;

namespace NArgsTest.PropertyServiceTests.Data
{
  public class DuplicateOptionNameConfiguration
  {
    [Option(Name = "o1")]
    public string Option1
    {
      get;
      set;
    }

    [Option(Name = "o1", LongName = "option2")]
    public string Option2
    {
      get;
      set;
    }

    [Option(Name = "o3")]
    public string Option3
    {
      get;
      set;
    }
  }
}
