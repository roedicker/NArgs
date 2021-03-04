using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NArgs;
using NArgs.Services;

using NArgsTest.PropertyServiceTests.Data;

namespace NArgsTest.PropertyServiceTests.DefaultPropertyServiceTests
{
  [TestClass]
  public class OptionTests
  {
    [TestMethod]
    public void Missing_Required_Option_Name_Should_Throw_Exception()
    {
      try
      {
        var target = new DefaultPropertyService(new MissingOptionNameConfiguration());
      }
      catch(InvalidConfigurationException ex)
      {
        Assert.AreEqual(@"Configuration is invalid. Option for property ""Option2"" is missing its required name", ex.Message);
      }
    }

    [TestMethod]
    public void Duplicate_Option_Name_Should_Throw_Exception()
    {
      try
      {
        var target = new DefaultPropertyService(new DuplicateOptionNameConfiguration());
      }
      catch (InvalidConfigurationException ex)
      {
        Assert.AreEqual(@"Configuration is invalid. Option name ""o1"" has already been used", ex.Message);
      }
    }

    [TestMethod]
    public void Duplicate_Option_Alternative_Name_Should_Throw_Exception()
    {
      try
      {
        var target = new DefaultPropertyService(new DuplicateOptionAlternativeNameConfiguration());
      }
      catch (InvalidConfigurationException ex)
      {
        Assert.AreEqual(@"Configuration is invalid. Option alternative name ""alt1"" has already been used", ex.Message);
      }
    }

    [TestMethod]
    public void Duplicate_Option_Long_Name_Should_Throw_Exception()
    {
      try
      {
        var target = new DefaultPropertyService(new DuplicateOptionLongNameConfiguration());
      }
      catch (InvalidConfigurationException ex)
      {
        Assert.AreEqual(@"Configuration is invalid. Option long name ""long1"" has already been used", ex.Message);
      }
    }
  }
}
