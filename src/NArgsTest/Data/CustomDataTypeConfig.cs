using System;

using NArgs.Attributes;

namespace NArgsTest.Data
{
  public class CustomDataTypeConfig
  {
    [OptionAttribute(LongName = "color-name", Name = "color")]
    public Color Color
    {
      get;
      set;
    }
  }
}
