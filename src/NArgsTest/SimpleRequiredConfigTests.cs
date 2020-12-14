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
      SimpleRequiredConfig oConfig = new SimpleRequiredConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      string expected = @"SYNTAX:
  Unit Test /required1 | --required-option1 <option>
            /required2 | --required-option2 <option>
            [/optional | --optional-option <option>]

OPTIONS:
  /required1 | --required-option1     Example required option #1
  /optional | --optional-option       Example optional option
  /required2 | --required-option2     Example required option #2
";

      string actual = oParser.GetUsage(oConfig, "Unit Test");

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Valid_Required_Option_Should_Be_Parsed()
    {
      SimpleRequiredConfig oConfig = new SimpleRequiredConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
      oParser.ParseArguments(oConfig, @"/required1:""abc"" -required2:123");

      Assert.AreEqual("abc", oConfig.RequiredOption1);
      Assert.AreEqual(123, oConfig.RequiredOption2);
    }

    [TestMethod]
    public void Missing_Required_Option1_Should_be_Parsed_With_Error()
    {
      SimpleRequiredConfig oConfig = new SimpleRequiredConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
      ParseResult actual;

      actual = oParser.ParseArguments(oConfig, @"-required2:123");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(1, actual.Errors.Count());

      ParseError oActualError = actual.Errors.First();

      Assert.AreEqual(ParseErrorType.RequiredOptionValue, oActualError.ErrorType);
      Assert.AreEqual("required1", oActualError.ItemName);
    }

    [TestMethod]
    public void Missing_Required_Option2_Shoud_be_Parsed_With_Error()
    {
      SimpleRequiredConfig oConfig = new SimpleRequiredConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
      ParseResult actual;

      actual = oParser.ParseArguments(oConfig, @"-required1 ""abc""");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(1, actual.Errors.Count());

      ParseError oActualError = actual.Errors.First();

      Assert.AreEqual(ParseErrorType.RequiredOptionValue, oActualError.ErrorType);
      Assert.AreEqual("required2", oActualError.ItemName);
    }

    [TestMethod]
    public void Missing_Required_Options_Should_Be_Parsed_With_Errors()
    {
      SimpleRequiredConfig oConfig = new SimpleRequiredConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
      ParseResult actual;
      ParseError oActualError;

      actual = oParser.ParseArguments(oConfig, @"");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(2, actual.Errors.Count());

      oActualError = actual.Errors.First();
      Assert.AreEqual(ParseErrorType.RequiredOptionValue, oActualError.ErrorType);
      Assert.AreEqual("required1", oActualError.ItemName);

      oActualError = actual.Errors.Last();
      Assert.AreEqual(ParseErrorType.RequiredOptionValue, oActualError.ErrorType);
      Assert.AreEqual("required2", oActualError.ItemName);
    }

    [TestMethod]
    public void Valid_Complex_Required_Option_Should_be_Parsed()
    {
      SimpleRequiredConfig oConfig = new SimpleRequiredConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      string expected = "http://192.168.1.2/root-path/";

      oConfig.RequiredOption1 = null;
      oParser.ParseArguments(oConfig, $@"-required1 ""{expected}"" -required2 123");
      Assert.AreEqual(expected, oConfig.RequiredOption1);
      Assert.AreEqual(123, oConfig.RequiredOption2);

      oConfig.RequiredOption1 = null;
      oParser.ParseArguments(oConfig, $@"-required1:""{expected}"" -required2:123");
      Assert.AreEqual(expected, oConfig.RequiredOption1);
      Assert.AreEqual(123, oConfig.RequiredOption2);

      oConfig.RequiredOption1 = null;
      oParser.ParseArguments(oConfig, $@"/required1 ""{expected}"" /required2 123");
      Assert.AreEqual(expected, oConfig.RequiredOption1);
      Assert.AreEqual(123, oConfig.RequiredOption2);

      oConfig.RequiredOption1 = null;
      oParser.ParseArguments(oConfig, $@"/required1:""{expected}"" /required2:123");
      Assert.AreEqual(expected, oConfig.RequiredOption1);
      Assert.AreEqual(123, oConfig.RequiredOption2);
    }
  }
}
