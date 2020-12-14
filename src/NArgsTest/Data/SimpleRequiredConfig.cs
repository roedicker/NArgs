using System;

using NArgs.Attributes;

namespace NArgsTest.Data
{
  public class SimpleRequiredConfig
  {
    [Option(Name = "required1", LongName = "required-option1", Description = "Example required option #1", Required = true)]
    public string RequiredOption1
    {
      get;
      set;
    }

    [Option(Name = "required2", LongName = "required-option2", Description = "Example required option #2", Required = true)]
    public int RequiredOption2
    {
      get;
      set;
    }


    [Option(Name = "optional", LongName = "optional-option", Description = "Example optional option")]
    public string OptionalOption
    {
      get;
      set;
    }
  }
}
