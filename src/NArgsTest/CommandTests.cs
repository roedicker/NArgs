using System;
using System.Linq;
using NArgs;
using NArgsTest.Data.Commands.Enums;
using NArgsTest.Data.Commands.Models;
using NUnit.Framework;
using Shouldly;

namespace NArgsTest;

[TestFixture]
public class CommandTests
{
    protected IArgumentParser Target
    {
        get;
        private set;
    }

    [OneTimeSetUp]
    public void Setup()
    {
        Target = new ConsoleCommandLineParser();
        Target.RegisterCustomDataTypeHandler(typeof(DateTimeCalculationType), (name, value) =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Missing required parameter value", nameof(value));
            }

            return value.ToLowerInvariant() switch
            {
                "date" => DateTimeCalculationType.CurrentDate,
                "datetime" or "date-time" => DateTimeCalculationType.CurrentDateTime,
                "time" => DateTimeCalculationType.CurrentTime,
                "year" => DateTimeCalculationType.CurrentYear,
                _ => (object)DateTimeCalculationType.None,
            };
        },

        (name, value, required) =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Missing required parameter value", nameof(value));
            }

            return new string[] { "none", "date", "datetime", "date-time", "time", "year" }.Contains(value.ToLowerInvariant());
        });
    }

    [Test]
    public void Valid_Simple_Command_Configuration_Should_Be_Parsed_Successfully()
    {
        var config = new CommandOnlyConfig();
        var result = Target.ParseArguments(config, "get date ./file.txt");

        result.Status.ShouldBe(ResultStatus.Success);
        config.Get.CalculationType.ShouldBe(DateTimeCalculationType.CurrentDate);
    }

    [Test]
    public void Valid_Command_Should_Trigger_Related_Action()
    {
        var commandName = "g";
        var config = new ComplexCommandConfig();
        var actualCommandName = string.Empty;
        object actualCommandObject = null;
        var result = Target.ParseArguments(config, $"{commandName} date", (name, command) => { actualCommandName = name; actualCommandObject = command; });

        result.Status.ShouldBe(ResultStatus.Success);
        actualCommandName.ShouldBe(commandName);
        actualCommandObject.ShouldNotBeNull();
    }

    [Test]
    public void Valid_Complex_Command_Configuration_Should_Be_Parsed_Successfully()
    {
        var config = new ComplexCommandConfig();
        var result = Target.ParseArguments(config, "set year ./file.txt");

        result.Status.ShouldBe(ResultStatus.Success);
        config.Set.CalculationType.ShouldBe(DateTimeCalculationType.CurrentYear);
    }

    [Test]
    public void NonExisting_Command_Should_Be_Parsed_With_Error()
    {
        var commandName = "remove";
        var config = new ComplexCommandConfig();
        var result = Target.ParseArguments(config, $"{commandName} year");

        result.Status.ShouldBe(ResultStatus.Failure);

        var actualError = result.Errors.First();
        actualError.ErrorType.ShouldBe(ParseErrorType.InvalidCommandArgsFormat);
        actualError.ItemName.ShouldBe(commandName);
    }

    [Test]
    public void Get_Usage_For_Command_Only_Configuration_Should_Be_Generated_As_Expected()
    {
        var executable = "UnitTest";
        var config = new CommandOnlyConfig();
        var expected = $@"SYNTAX:
  {executable} <command> [<args>]

COMMANDS:
  g | get     Gets information about date and time
";

        var actual = Target.GetUsage(config, executable);

        actual.ShouldBe(expected);
    }

    [Test]
    public void Get_Usage_For_Complex_Command_Configuration_Should_Be_Generated_As_Expected()
    {
        var executable = "UnitTest";
        var config = new ComplexCommandConfig();
        var expected = $@"SYNTAX:
  {executable} [/h | /? | --help]
           [/v | --verbose]
           <command> [<args>]

OPTIONS:
  /h | /? | --help     n/a
  /v | --verbose       Indicator whether output should be verbose

COMMANDS:
  g | get     Gets information about date and time
  s | set     Sets information about date and time
";

        var actual = Target.GetUsage(config, executable);

        actual.ShouldBe(expected);
    }

    [Test]
    public void Get_Usage_For_Short_Named_Command_Of_Command_Only_Configuration_Should_Be_Generated_As_Expected()
    {
        var executable = "UnitTest";
        var commandName = "g";
        var config = new CommandOnlyConfig();
        var expected = $@"SYNTAX:
  {executable} {commandName} <calculation-type> <data-source> [/utc | --use-utc]

PARAMETERS:
  calculation-type     Determines what kind of date-time to be calculated
  data-source          Gets / sets the data source to get and set data

OPTIONS:
  /utc | --use-utc     Indicator whether to use UTC based date-time information
";

        var actual = Target.GetUsage(config, executable, commandName);

        actual.ShouldBe(expected);
    }

    [Test]
    public void Get_Usage_For_Long_Named_Command_Of_Command_Only_Configuration_Should_Be_Generated_As_Expected()
    {
        var executable = "UnitTest";
        var commandName = "get";
        var config = new CommandOnlyConfig();
        var expected = $@"SYNTAX:
  {executable} {commandName} <calculation-type> <data-source> [/utc | --use-utc]

PARAMETERS:
  calculation-type     Determines what kind of date-time to be calculated
  data-source          Gets / sets the data source to get and set data

OPTIONS:
  /utc | --use-utc     Indicator whether to use UTC based date-time information
";

        var actual = Target.GetUsage(config, executable, commandName);

        actual.ShouldBe(expected);
    }

    [Test]
    public void Get_Usage_For_Short_Named_Command_Of_Complex_Command_Configuration_Should_Be_Generated_As_Expected()
    {
        var executable = "UnitTest";
        var commandName = "s";
        var config = new ComplexCommandConfig();
        var expected = $@"SYNTAX:
  {executable} {commandName} <calculation-type> <data-source> [/utc | --use-utc]

PARAMETERS:
  calculation-type     Determines what kind of date-time to be calculated
  data-source          Gets / sets the data source to get and set data

OPTIONS:
  /utc | --use-utc     Indicator whether to use UTC based date-time information
";

        var actual = Target.GetUsage(config, executable, commandName);

        actual.ShouldBe(expected);
    }

    [Test]
    public void Get_Usage_For_Long_Named_Command_Of_Complex_Command_Configuration_Should_Be_Generated_As_Expected()
    {
        var executable = "UnitTest";
        var commandName = "set";
        var config = new ComplexCommandConfig();
        var expected = $@"SYNTAX:
  {executable} {commandName} <calculation-type> <data-source> [/utc | --use-utc]

PARAMETERS:
  calculation-type     Determines what kind of date-time to be calculated
  data-source          Gets / sets the data source to get and set data

OPTIONS:
  /utc | --use-utc     Indicator whether to use UTC based date-time information
";

        var actual = Target.GetUsage(config, executable, commandName);

        actual.ShouldBe(expected);
    }
}
