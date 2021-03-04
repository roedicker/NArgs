using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NArgs.Attributes;

namespace NArgsTest.PropertyServiceTests.Data
{
  public class InvalidParameterOrdinalNumberSequenceConfiguration
  {
    [Parameter(Name = "p1", OrdinalNumber = 3)]
    public string Parameter1
    {
      get;
      set;
    }

    [Parameter(Name = "p2", OrdinalNumber = 1)]
    public string Parameter2
    {
      get;
      set;
    }

    [Parameter(Name = "p3", OrdinalNumber = 4)]
    public string Parameter3
    {
      get;
      set;
    }
  }
}
