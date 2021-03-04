using System;

using NArgs.Attributes;

namespace NArgsTest.PropertyServiceTests.Data
{
  public class DuplicateOptionLongNameConfiguration
  {
    [Option(Name = "o1", AlternativeName = "alt1", LongName = "long1")]
    public string Option1
    {
      get;
      set;
    }

    [Option(Name = "o2", AlternativeName = "alt2", LongName = "long1")]
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
