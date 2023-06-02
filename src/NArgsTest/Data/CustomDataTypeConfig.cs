using NArgs.Attributes;

namespace NArgsTest.Data;

public class CustomDataTypeConfig
{
[Option(LongName = "color-name", Name = "color")]
public Color Color
{
  get;
  set;
}
}
