using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NArgs.Attributes;

namespace NArgsTest.PropertyServiceTests.Data
{
  public class DuplicateParameterNameConfiguration
  {
    [ParameterAttribute(OrdinalNumber = 1)]
    public string Parameter1
    {
      get;
      set;
    }

    [ParameterAttribute(Name = "p2", OrdinalNumber = 2)]
    public string Parameter2
    {
      get;
      set;
    }

    [ParameterAttribute(Name = "p2", OrdinalNumber = 3)]
    public string Parameter3
    {
      get;
      set;
    }
  }
}
