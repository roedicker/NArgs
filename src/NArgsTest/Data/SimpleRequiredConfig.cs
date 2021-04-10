using System;

using NArgs.Attributes;

namespace NArgsTest.Data
{
  public class SimpleRequiredConfig
  {
    [OptionAttribute(Name = "R1", LongName = "required-option1", Description = "Example required option #1", Required = true)]
    public string RequiredOption1
    {
      get;
      set;
    }

    [OptionAttribute(Name = "R2", LongName = "required-option2", Description = "Example required option #2", Required = true)]
    public int RequiredOption2
    {
      get;
      set;
    }


    [OptionAttribute(Name = "O", LongName = "optional-option", Description = "Example optional option")]
    public string OptionalOption
    {
      get;
      set;
    }
  }
}
