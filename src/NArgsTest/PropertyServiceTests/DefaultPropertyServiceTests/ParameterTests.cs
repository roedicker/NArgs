using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NArgs;
using NArgs.Attributes;
using NArgs.Services;
using NArgsTest.PropertyServiceTests.Data;

namespace NArgsTest.PropertyServiceTests.DefaultPropertyServiceTests
{
  [TestClass]
  public class ParameterTests
  {
    [TestMethod]
    public void Duplicate_Parameter_Ordinal_Number_Throws_Exception()
    {
      try
      {
        var target = new DefaultPropertyService(new DuplicateParameterOrdinalNumberConfiguration());
      }
      catch (InvalidConfigurationException ex)
      {
        Assert.AreEqual("Configuration is invalid. Parameter ordinal number 2 has already been used", ex.Message);
      }
    }

    [TestMethod]
    public void Duplicate_Parameter_Name_Throws_Exception()
    {
      try
      {
        var target = new DefaultPropertyService(new DuplicateParameterNameConfiguration());
      }
      catch (InvalidConfigurationException ex)
      {
        Assert.AreEqual(@"Configuration is invalid. Parameter name ""p2"" has already been used", ex.Message);
      }
    }

    [TestMethod]
    public void Invalid_Parameter_Ordinal_Number_Throws_Exception()
    {
      try
      {
        var target = new DefaultPropertyService(new InvalidParameterOrdinalNumberConfiguration());
      }
      catch (Exception ex)
      {
        ArgumentOutOfRangeException baseException = ex.GetBaseException() as ArgumentOutOfRangeException;

        Assert.IsNotNull(baseException);
        Assert.AreEqual(baseException.ParamName, "OrdinalNumber");
      }
    }

    [TestMethod]
    public void Invalid_Parameter_Ordinal_Number_Sequence_Throws_Exception()
    {
      try
      {
        var target = new DefaultPropertyService(new InvalidParameterOrdinalNumberSequenceConfiguration());
      }
      catch (InvalidConfigurationException ex)
      {
        Assert.AreEqual("Configuration is invalid. Parameter ordinal numbers are not used in sequence", ex.Message);
      }
    }
  }
}
