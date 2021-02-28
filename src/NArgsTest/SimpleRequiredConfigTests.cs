using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NArgs;
using NArgs.Models;

using NArgsTest.Data;

namespace NArgsTest
{
  [TestClass]
  public class SimpleRequiredConfigTests
  {
    [TestMethod]
    public void Get_Usage_Via_Interface_Should_Be_Valid()
    {
      var target = new ConsoleCommandLineParser(new SimpleRequiredConfig());
      var expected = @"SYNTAX:
  Unit Test /required1 | --required-option1 <option>
            /required2 | --required-option2 <option>
            [/optional | --optional-option <option>]

OPTIONS:
  /required1 | --required-option1     Example required option #1
  /optional | --optional-option       Example optional option
  /required2 | --required-option2     Example required option #2
";

      var actual = target.GetUsage("Unit Test");

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Valid_Required_Option_Should_Be_Parsed()
    {
      var data = new SimpleRequiredConfig();
      var target = new ConsoleCommandLineParser(data);
      target.ParseArguments(@"/required1:""abc"" -required2:123");

      Assert.AreEqual("abc", data.RequiredOption1);
      Assert.AreEqual(123, data.RequiredOption2);
    }

    [TestMethod]
    public void Missing_Required_Option1_Should_be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleRequiredConfig());
      var actual = target.ParseArguments(@"-required2:123");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(1, actual.Errors.Count());

      var error = actual.Errors.First();

      Assert.AreEqual(ParseErrorType.RequiredOptionValue, error.ErrorType);
      Assert.AreEqual("required1", error.ItemName);
    }

    [TestMethod]
    public void Missing_Required_Option2_Shoud_be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleRequiredConfig());
      var actual = target.ParseArguments(@"-required1 ""abc""");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(1, actual.Errors.Count());

      var error = actual.Errors.First();

      Assert.AreEqual(ParseErrorType.RequiredOptionValue, error.ErrorType);
      Assert.AreEqual("required2", error.ItemName);
    }

    [TestMethod]
    public void Missing_Required_Options_Should_Be_Parsed_With_Errors()
    {
      var target = new ConsoleCommandLineParser(new SimpleRequiredConfig());
      var actual = target.ParseArguments(@"");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(2, actual.Errors.Count());

      var error = actual.Errors.First();
      Assert.AreEqual(ParseErrorType.RequiredOptionValue, error.ErrorType);
      Assert.AreEqual("required1", error.ItemName);

      error = actual.Errors.Last();
      Assert.AreEqual(ParseErrorType.RequiredOptionValue, error.ErrorType);
      Assert.AreEqual("required2", error.ItemName);
    }

    [TestMethod]
    public void Valid_Complex_Required_Option_Should_be_Parsed()
    {
      var data = new SimpleRequiredConfig();
      var  target = new ConsoleCommandLineParser(data);
      var expected = "http://192.168.1.2/root-path/";

      data.RequiredOption1 = null;
      target.ParseArguments($@"-required1 ""{expected}"" -required2 123");
      Assert.AreEqual(expected, data.RequiredOption1);
      Assert.AreEqual(123, data.RequiredOption2);

      data.RequiredOption1 = null;
      target.ParseArguments($@"-required1:""{expected}"" -required2:123");
      Assert.AreEqual(expected, data.RequiredOption1);
      Assert.AreEqual(123, data.RequiredOption2);

      data.RequiredOption1 = null;
      target.ParseArguments($@"/required1 ""{expected}"" /required2 123");
      Assert.AreEqual(expected, data.RequiredOption1);
      Assert.AreEqual(123, data.RequiredOption2);

      data.RequiredOption1 = null;
      target.ParseArguments($@"/required1:""{expected}"" /required2:123");
      Assert.AreEqual(expected, data.RequiredOption1);
      Assert.AreEqual(123, data.RequiredOption2);
    }
  }
}
