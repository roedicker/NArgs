using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NArgs;
using NArgsTest.Data;
using NUnit.Framework;
using Shouldly;

namespace NArgsTest;

[TestFixture]
public class SimpleValidConfigTests
{
    [Test]
    public void Valid_Bool_Options_Should_Be_Parsed()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, "--help -v:yes");

        result.Status.ShouldBe(ResultStatus.Success);
        config.ShowHelpOption.ShouldBeTrue();
        config.VerboseMessagesOption.ShouldBeTrue();
    }

    [Test]
    public void Valid_Bool_Option_With_AlternativeName_Should_Be_Parsed()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, "/?");

        result.Status.ShouldBe(ResultStatus.Success);
        config.ShowHelpOption.ShouldBeTrue();
    }

    [Test]
    public void Valid_Bool_Option_With_AlternativeName_And_Value_Should_be_Parsed()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, "/?:yes");

        result.Status.ShouldBe(ResultStatus.Success);
        config.ShowHelpOption.ShouldBeTrue();
    }

    [Test]
    public void Valid_Bool_Option_With_AlternativeName_And_Alternative_Value_Should_be_Parsed()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, "/? false");

        result.Status.ShouldBe(ResultStatus.Success);
        config.ShowHelpOption.ShouldBeFalse();
    }

    [Test]
    public void Invalid_Bool_Option_Should_Be_Parsed_With_Error()
    {
        var optionValue = "Nein";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $"/h {optionValue}");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
        result.Errors.First().Message.ShouldBe($@"Value ""{optionValue}"" is invalid for option ""h""");
    }

    [Test]
    public void Invalid_Bool_Option_With_LongName_Should_Be_Parsed_With_Error()
    {
        var optionValue = "Nein";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $"--help {optionValue}");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
        result.Errors.First().Message.ShouldBe($@"Value ""{optionValue}"" is invalid for option ""help""");
    }

    [Test]
    public void Invalid_Bool_Option_With_AlternativeName_Should_Be_Parsed_With_Error()
    {
        var optionValue = "Nein";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $"/? {optionValue}");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
        result.Errors.First().Message.ShouldBe($@"Value ""{optionValue}"" is invalid for option ""?""");
    }

    [Test]
    [TestCase('a')]
    [TestCase('A')]
    [TestCase('1')]
    public void Valid_Char_Option_Value_Should_Be_Parsed(char value)
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $"-c {value}");

        result.Status.ShouldBe(ResultStatus.Success);
        config.CharValueOption.ShouldBe(value);
    }

    [Test]
    [TestCase("abc")]
    [TestCase(123)]
    public void Invalid_Char_Option_With_Invalid_Value_Should_Be_Parsed_With_Error(object value)
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $"-c {value}");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }

    [Test]
    public void Valid_String_Option_Should_Be_Parsed()
    {
        var optionValue = "Example Name";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-s:""{optionValue}""");

        result.Status.ShouldBe(ResultStatus.Success);
        config.StringValueOption.ShouldBe(optionValue);
    }

    [Test]
    public void Valid_Int16_Option_Should_Be_Parsed()
    {
        var optionValue = (short)32767;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-i16 {optionValue}");

        result.Status.ShouldBe(ResultStatus.Success);
        config.Int16ValueOption.ShouldBe(optionValue);
    }

    [Test]
    public void Invalid_Int16_Option_Should_Be_Parsed_With_Error()
    {
        var optionValue = 32768;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-i16 {optionValue}");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }

    [Test]
    public void Valid_Int32_Option_Should_Be_Parsed()
    {
        var optionValue = 32768;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-i {optionValue}");

        result.Status.ShouldBe(ResultStatus.Success);
        config.Int32ValueOption.ShouldBe(optionValue);
    }

    [Test]
    [TestCase(1.23D)]
    [TestCase("abc")]
    public void Invalid_Int32_Option_Should_Be_Parsed_With_Error(object value)
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-i {value}");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }

    [Test]
    public void Valid_Int64_Option_Should_Be_Parsed()
    {
        var optionValue = 1234567890L;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-l {optionValue}");

        result.Status.ShouldBe(ResultStatus.Success);
        config.Int64ValueOption.ShouldBe(optionValue);
    }

    [Test]
    [TestCase(1.23D)]
    [TestCase("abc")]
    public void Invalid_Int64_Option_Should_Be_Parsed_With_Error(object value)
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-l {value}");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }

    [Test]
    public void Valid_DateTime_Option_Should_Be_Parsed()
    {
        var optionValue = "1970-04-01 10:43:28";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"--date ""{optionValue}""");

        result.Status.ShouldBe(ResultStatus.Success);
        config.DateValueOption.ShouldBe(DateTime.Parse(optionValue));
    }

    [Test]
    public void Valid_Double_Option_Should_Be_Parsed()
    {
        var optionValue = 3.14159265358979D;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, string.Create(parser.Options.Culture, $@"--double {optionValue}"));

        result.Status.ShouldBe(ResultStatus.Success);
        config.DoubleValueOption.ShouldBe(optionValue);
    }

    [Test]
    public void Invalid_Double_Option_Should_Be_Parsed_With_Error()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"--double a");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }

    [Test]
    public void Valid_FileInfo_Option_Should_Be_Parsed()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, @"-file ""C:\Windows\explorer.exe""");

        result.Status.ShouldBe(ResultStatus.Success);
        config.FileInfoValueOption.ShouldNotBeNull();
    }

    [Test]
    public void Valid_Directory_Info_Option_Should_Be_Parsed()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, @"-dir "".\""");

        result.Status.ShouldBe(ResultStatus.Success);
        config.DirInfoValueOption.ShouldNotBeNull();
    }

    [Test]
    public void Valid_Parameters_Should_Be_Parsed()
    {
        var firstParameterValue = "First Parameter Value";
        var secondParameterValue = "Second Parameter Value";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"""{firstParameterValue}"" ""{secondParameterValue}""");

        result.Status.ShouldBe(ResultStatus.Success);
        config.TextValueParameter1.ShouldBe(firstParameterValue);
        config.TextValueParameter2.ShouldBe(secondParameterValue);
    }

    [Test]
    public void Valid_Mixed_Arguments_With_Correct_Sequence_Should_Be_Parsed()
    {
        var firstParameterValue = "First Parameter Value";
        var secondParameterValue = "Second Parameter Value";
        var optionValue = 123;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"""{firstParameterValue}"" ""{secondParameterValue}"" --verbose:yes -i:{optionValue}");

        result.Status.ShouldBe(ResultStatus.Success);
        config.TextValueParameter1.ShouldBe(firstParameterValue);
        config.TextValueParameter2.ShouldBe(secondParameterValue);
        config.VerboseMessagesOption.ShouldBeTrue();
        config.Int32ValueOption.ShouldBe(optionValue);
    }

    [Test]
    public void Invalid_Mixed_Arguments_With_Incorrect_Sequence_Should_Be_Parsed_With_Error()
    {
        var firstParameterValue = "First Parameter Value";
        var secondParameterValue = "Second Parameter Value";
        var optionName = "verbose";
        var optionValue = 123;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"--{optionName} ""{firstParameterValue}"" ""{secondParameterValue}"" -i {optionValue} ");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.Count().ShouldBe(2);

        var actualError = result.Errors.First();
        actualError.ErrorType.ShouldBe(ParseErrorType.InvalidCommandArgsFormat);
        actualError.ItemName.ShouldBe(secondParameterValue);

        actualError = result.Errors.Last();
        actualError.ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
        actualError.ItemName.ShouldBe(optionName);
        actualError.ItemValue.ShouldBe(firstParameterValue);
    }

    [Test]
    public void Invalid_Mixed_Arguments_With_Alternative_Incorrect_Sequence_Should_Be_Parsed_With_Error()
    {
        var firstParameterValue = "First Parameter Value";
        var secondParameterValue = "Second Parameter Value";
        var optionName = "verbose";
        var optionValue = 123;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"""{firstParameterValue}"" /i:{optionValue} --{optionName} ""{secondParameterValue}""");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.Count().ShouldBe(1);

        var actualError = result.Errors.First();
        actualError.ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
        actualError.ItemName.ShouldBe(optionName);
        actualError.ItemValue.ShouldBe(secondParameterValue);
    }

    [Test]
    public void Invalid_FileInfo_Option_Should_Be_Parsed_With_Error()
    {
        var optionName = "file";
        var optionValue = "?;";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-{optionName} ""{optionValue}""");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.Count().ShouldBe(1);

        var actualError = result.Errors.First();
        actualError.ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
        actualError.ItemName.ShouldBe(optionName);
        actualError.ItemValue.ShouldBe(optionValue);
    }

    [Test]
    public void Valid_Custom_DataType_Option_Should_Be_Parsed()
    {
        const string none = "None";
        const string red = "Red";
        const string yellow = "Yellow";
        const string green = "Green";
        var optionValue = Color.Green;
        var config = new CustomDataTypeConfig();
        var parser = new ConsoleCommandLineParser();

        parser.RegisterCustomDataTypeHandler(typeof(Color), (name, value) =>
        {
            return value switch
            {
                red => Color.Red,
                yellow => Color.Yellow,
                green => Color.Green,
                _ => Color.None,
            };
        },

        (name, value, required) =>
        {
            return new string[] { none, red, yellow, green }.Contains(value);
        });

        var result = parser.ParseArguments(config, $@"-color ""{optionValue}""");

        result.Status.ShouldBe(ResultStatus.Success);
        config.Color.ShouldBe(optionValue);
    }

    [Test]
    public void Valid_Single_Option_Should_Be_Parsed()
    {
        var optionValue = (float)3.402823E+38;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();

        // set / restore current thread's culture due to culture-depending notation
        var currentCulture = Thread.CurrentThread.CurrentCulture;

        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            var result = parser.ParseArguments(config, $@"--single {optionValue}");

            result.Status.ShouldBe(ResultStatus.Success);
            config.SingleValueOption.ShouldBe(optionValue);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }

    [Test]
    public void Invalid_Single_Option_With_Exceeding_Value_Should_Be_Parsed_As_Infinity()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, string.Create(parser.Options.Culture, $@"--single {3.402823E+39}"));

        result.Status.ShouldBe(ResultStatus.Success);
        float.IsInfinity(config.SingleValueOption).ShouldBeTrue();
    }

    [Test]
    public void Parse_Valid_UInt16_Long_Option()
    {
        var optionValue = (ushort)65535;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-ui16 {optionValue}");

        result.Status.ShouldBe(ResultStatus.Success);
        config.UInt16ValueOption.ShouldBe(optionValue);
    }

    [Test]
    public void Invalid_UInt16_With_Exceeding_Value_Option_Should_Be_Parsed_With_Error()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, @"-ui16 65536");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }

    [Test]
    public void Valid_UInt32_Option_Should_Be_Parsed()
    {
        var optionValue = (ushort)65535;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-ui32 {optionValue}");

        result.Status.ShouldBe(ResultStatus.Success);
        config.UInt32ValueOption.ShouldBe(optionValue);
    }

    [Test]
    public void Invalid_UInt32_With_Exceeding_Value_Option_Should_Be_Parsed_With_Error()
    {
        var optionValue = 4294967296;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-ui32 {optionValue}");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }

    [Test]
    public void Valid_UInt64_Option_Should_Be_Parsed()
    {
        var optionValue = (ulong)131072;
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"-ui64 {optionValue}");

        result.Status.ShouldBe(ResultStatus.Success);
        config.UInt64ValueOption.ShouldBe(optionValue);
    }

    [Test]
    public void Invalid_UInt64_Option_Should_Be_Parsed_With_Error()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, @"-ui64 -1");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }

    [Test]
    public void Complex_String_Option_Should_Be_Parsed()
    {
        var optionValue = "http://192.168.1.2/root-path/";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();

        config.StringValueOption = null;
        var result = parser.ParseArguments(config, $@"-s ""{optionValue}""");
        result.Status.ShouldBe(ResultStatus.Success);
        config.StringValueOption.ShouldBe(optionValue);

        config.StringValueOption = null;
        result = parser.ParseArguments(config, $@"-s:""{optionValue}""");
        result.Status.ShouldBe(ResultStatus.Success);
        config.StringValueOption.ShouldBe(optionValue);

        config.StringValueOption = null;
        result = parser.ParseArguments(config, $@"/s ""{optionValue}""");
        result.Status.ShouldBe(ResultStatus.Success);
        config.StringValueOption.ShouldBe(optionValue);

        config.StringValueOption = null;
        result = parser.ParseArguments(config, $@"/s:""{optionValue}""");
        result.Status.ShouldBe(ResultStatus.Success);
        config.StringValueOption.ShouldBe(optionValue);
    }

    [Test]
    public void Valid_Uri_Option_Should_Be_Parsed()
    {
        var optionValue = "http://example.com/root/";
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"/uri ""{optionValue}""");

        result.Status.ShouldBe(ResultStatus.Success);
        config.UriOptionValue.ToString().ShouldBe(optionValue);
    }

    [Test]
    public void Invalid_Uri_Option_Should_Be_Parsed_With_Error()
    {
        var config = new SimpleValidConfig();
        var parser = new ConsoleCommandLineParser();
        var result = parser.ParseArguments(config, $@"/uri ""C:\\Root\\Folder""");

        result.Status.ShouldBe(ResultStatus.Failure);
        result.Errors.First().ErrorType.ShouldBe(ParseErrorType.InvalidOptionValue);
    }
}
