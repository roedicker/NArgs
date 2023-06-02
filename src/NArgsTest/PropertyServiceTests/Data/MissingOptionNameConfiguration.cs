using System;

using NArgs.Attributes;

namespace NArgsTest.PropertyServiceTests.Data
{
  public class MissingOptionNameConfiguration
  {
    [OptionAttribute(Name = "o1")]
    public string Option1
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "option2")]
    public string Option2
    {
      get;
      set;
    }

    [OptionAttribute(Name = "o3")]
    public string Option3
    {
      get;
      set;
    }
  }
}
