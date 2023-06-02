using System.Linq;
using NArgs;
using NArgsTest.Data;
using NUnit.Framework;
using Shouldly;

namespace NArgsTest;

[TestFixture]
public class SimpleRequiredConfigTests
{
    [Test]
    public void Get_Usage_Via_Interface_Should_Be_Valid()
    {
        var config = new SimpleRequiredConfig();
        var parser = new ConsoleCommandLineParser();
        var expected = @"SYNTAX:
  Unit Test /required1 | --required-option1 <option>
            /required2 | --required-option2 <option>
            [/optional | --optional-option <option>]

OPTIONS:
  /required1 | --required-option1     Example required option #1
  /required2 | --required-option2     Example required option #2
  /optional | --optional-option       Example optional option
";

        var actual = parser.GetUsage(config, "Unit Test");

        expected.ShouldBe(actual);
    }

    [Test]
    public void Valid_Required_Option_Should_Be_Parsed()
    {
        var optionValue1 = "abc";
        var optionValue2 = 123;
        var config = new SimpleRequiredConfig();
        var parser = new ConsoleCommandLineParser();
        parser.ParseArguments(config, $@"/required1:""{optionValue1}"" -required2:{optionValue2}");

        config.RequiredOption1.ShouldBe(optionValue1);
        config.RequiredOption2.ShouldBe(optionValue2);
    }

    [Test]
    public void Missing_Required_Option1_Should_be_Parsed_With_Error()
    {
        var optionValue = 123;
        var config = new SimpleRequiredConfig();
        var parser = new ConsoleCommandLineParser();
        var actual = parser.ParseArguments(config, $@"-required2:{optionValue}");

        actual.Status.ShouldBe(ResultStatus.Failure);
        actual.Errors.Count().ShouldBe(1);

        var actualError = actual.Errors.First();

        actualError.ErrorType.ShouldBe(ParseErrorType.RequiredOptionValue);
        actualError.ItemName.ShouldBe("required1");
    }

    [Test]
    public void Missing_Required_Option2_Should_be_Parsed_With_Error()
    {
        var optionValue = "abc";
        var config = new SimpleRequiredConfig();
        var parser = new ConsoleCommandLineParser();
        var actual = parser.ParseArguments(config, $@"-required1 ""{optionValue}""");

        actual.Status.ShouldBe(ResultStatus.Failure);
        actual.Errors.Count().ShouldBe(1);

        var actualError = actual.Errors.First();

        actualError.ErrorType.ShouldBe(ParseErrorType.RequiredOptionValue);
        actualError.ItemName.ShouldBe("required2");
    }

    [Test]
    public void Missing_Required_Options_Should_Be_Parsed_With_Errors()
    {
        var config = new SimpleRequiredConfig();
        var parser = new ConsoleCommandLineParser();
        var actual = parser.ParseArguments(config, @"");

        Assert.AreEqual(ResultStatus.Failure, actual.Status);
        Assert.AreEqual(2, actual.Errors.Count());

        var actualError = actual.Errors.First();
        actualError.ErrorType.ShouldBe(ParseErrorType.RequiredOptionValue);
        actualError.ItemName.ShouldBe("required1");

        actualError = actual.Errors.Last();
        actualError.ErrorType.ShouldBe(ParseErrorType.RequiredOptionValue);
        actualError.ItemName.ShouldBe("required2");
    }

    [Test]
    public void Valid_Complex_Required_Option_Should_be_Parsed()
    {
        var optionValue1 = "http://192.168.1.2/root-path/";
        var optionValue2 = 123;
        var config = new SimpleRequiredConfig();
        var parser = new ConsoleCommandLineParser();

        config.RequiredOption1 = null;
        parser.ParseArguments(config, $@"-required1 ""{optionValue1}"" -required2 {optionValue2}");
        config.RequiredOption1.ShouldBe(optionValue1);
        config.RequiredOption2.ShouldBe(optionValue2);

        config.RequiredOption1 = null;
        parser.ParseArguments(config, $@"-required1:""{optionValue1}"" -required2:{optionValue2}");
        config.RequiredOption1.ShouldBe(optionValue1);
        config.RequiredOption2.ShouldBe(optionValue2);

        config.RequiredOption1 = null;
        parser.ParseArguments(config, $@"/required1 ""{optionValue1}"" /required2 {optionValue2}");
        config.RequiredOption1.ShouldBe(optionValue1);
        config.RequiredOption2.ShouldBe(optionValue2);

        config.RequiredOption1 = null;
        parser.ParseArguments(config, $@"/required1:""{optionValue1}"" /required2:123");
        config.RequiredOption1.ShouldBe(optionValue1);
        config.RequiredOption2.ShouldBe(optionValue2);
    }
}
