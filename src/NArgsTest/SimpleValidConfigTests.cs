using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NArgs;
using NArgs.Models;

using NArgsTest.Data;

namespace NArgsTest
{
  [TestClass]
  public class SimpleValidConfigTests
  {
    [TestMethod]
    public void Valid_Bool_Options_Should_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("--help -v:yes");

      Assert.AreEqual(true, data.ShowHelpOption);
      Assert.AreEqual(true, data.VerboseMessagesOption);
    }

    [TestMethod]
    public void Valid_Bool_Option_With_AlternativeName_Should_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("/?");

      Assert.AreEqual(true, data.ShowHelpOption);
    }

    [TestMethod]
    public void Valid_Bool_Option_With_AlternativeName_And_Value_Should_be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("/?:yes");

      Assert.AreEqual(true, data.ShowHelpOption);
    }

    [TestMethod]
    public void Valid_Bool_Option_With_AlternativeName_And_Alternative_Value_Should_be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("/? false");

      Assert.AreEqual(false, data.ShowHelpOption);
    }

    [TestMethod]
    public void Invalid_Bool_Option_Should_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments("/h:Nein");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
      Assert.AreEqual(actual.Errors.First().ItemName, "h");
    }

    [TestMethod]
    public void Invalid_Bool_Option_With_LongName_Should_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments("--help= Nein");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
      Assert.AreEqual($@"Value ""Nein"" is invalid for option ""help""", actual.Errors.First().Message);
    }

    [TestMethod]
    public void Invalid_Bool_Option_With_AlternativeName_Should_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments("/?: Nein");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
      Assert.AreEqual(actual.Errors.First().ItemName, "?");
    }

    [TestMethod]
    public void Valid_Char_Option_With_Lowercase_Value_Should_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("-c a");

      Assert.AreEqual('a', data.CharValueOption);
    }

    [TestMethod]
    public void Valid_Char_Option_With_Uppercase_Value_Should_be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("-c A");

      Assert.AreEqual('A', data.CharValueOption);
    }

    [TestMethod]
    public void Valid_Char_Option_With_Single_Numeric_Value_Should_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("-c 1");

      Assert.AreEqual('1', data.CharValueOption);
    }

    [TestMethod]
    public void Invalid_Char_Option_With_String_Value_Should_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments("-c abc");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Invalid_Char_Option_With_Long_Numeric_Value_Should_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments("-c: 123");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_String_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"-s:""Example Name""");

      Assert.AreEqual("Example Name", data.StringValueOption);
    }

    [TestMethod]
    public void Valid_Int16_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"-i16 32767");

      Assert.AreEqual(32767, data.Int16ValueOption);
    }

    [TestMethod]
    public void Invalid_Int16_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments(@"-i16 32768");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_Int32_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"-i 123");

      Assert.AreEqual(123, data.Int32ValueOption);
    }

    [TestMethod]
    public void Invalid_Int32_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments(@"-i 1.0");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_Int64_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"-l 1234567890");

      Assert.AreEqual(1234567890, data.Int64ValueOption);
    }

    [TestMethod]
    public void Invalid_Int64_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments(@"-l abc");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_DateTime_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"--date ""1970-04-01 10:43:28""");

      Assert.AreEqual(DateTime.Parse("1970-04-01 10:43:28"), data.DateValueOption);
    }

    [TestMethod]
    public void Valid_Double_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);
      double value = 3.14159265358979;

      target.ParseArguments($@"--double {value.ToString(target.Options.Culture)}");

      Assert.AreEqual(value, data.DoubleValueOption);
    }

    [TestMethod]
    public void Invalid_Double_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments($@"--double a");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_FileInfo_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"-file ""C:\Windows\explorer.exe""");

      Assert.IsNotNull(data.FileInfoValueOption);
    }

    [TestMethod]
    public void Valid_Directory_Info_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"-dir ""C:\Windows""");

      Assert.IsNotNull(data.DirInfoValueOption);
    }

    [TestMethod]
    public void Valid_Parameters_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"""First Parameter Value"" ""Second Parameter Value""");

      Assert.AreEqual("First Parameter Value", data.TextValueParameter1);
      Assert.AreEqual("Second Parameter Value", data.TextValueParameter2);
    }

    [TestMethod]
    public void Valid_Mixed_Arguments_With_Correct_Sequence_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments(@"""First Parameter Value"" ""Second Parameter Value"" --verbose:yes -i:123");

      Assert.AreEqual("First Parameter Value", data.TextValueParameter1);
      Assert.AreEqual("Second Parameter Value", data.TextValueParameter2);
      Assert.AreEqual(true, data.VerboseMessagesOption);
      Assert.AreEqual(123, data.Int32ValueOption);
    }

    [TestMethod]
    [DataRow(@"--verbose ""First Parameter Value"" ""Second Parameter Value"" -i 123 ")]
    [DataRow(@"""First Parameter Value"" /i:123 --verbose ""Second Parameter Value""")]
    public void Mixed_Arguments_Shoud_Be_Parsed(string args)
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments(args);

      Assert.AreEqual(ResultStatus.Success, actual.Status);
    }

    [TestMethod]
    public void Invalid_FileInfo_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments(@"-file: ""?;""");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(1, actual.Errors.Count());

      var error = actual.Errors.First();

      Assert.AreEqual(ParseErrorType.InvalidOptionValue, error.ErrorType);
      Assert.AreEqual("file", error.ItemName);
      Assert.AreEqual("?;", error.ItemValue);
    }

    [TestMethod]
    public void Valid_Custom_DataType_Option_Shoud_Be_Parsed()
    {
      var data = new CustomDataTypeConfig();
      var target = new ConsoleCommandLineParser(data);

      target.RegisterCustomDataTypeHandler(typeof(Color), (name, value) =>
      {
        switch (value)
        {
          case "Red":
            return Color.Red;

          case "Yellow":
            return Color.Yellow;

          case "Green":
            return Color.Green;

          default:
            return Color.None;
        }
      },

      (name, value, required) =>
      {
        return new string[] { "None", "Red", "Yellow", "Green" }.Contains(value);
      });

      target.ParseArguments(@"-color ""Green""");

      Assert.AreEqual(Color.Green, data.Color);
    }

    [TestMethod]
    public void Valid_Single_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);
      var value = (float)3.402823E+38;

      target.ParseArguments($@"--single {value.ToString(target.Options.Culture)}");

      Assert.AreEqual(value, data.SingleValueOption);
    }

    [TestMethod]
    public void Invalid_Single_Option_With_Exceeding_Value_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());

      var actual = target.ParseArguments($@"--single x1");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Parse_Valid_UInt16_Long_Option()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);
      ushort value = 65535;

      target.ParseArguments($"-ui16 {value}");

      Assert.AreEqual(value, data.UInt16ValueOption);
    }

    [TestMethod]
    public void Invalid_UInt16_With_Exceeding_Value_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments("-ui16 65536");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_UInt32_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);
      uint value = 65536;

      target.ParseArguments($"-ui32 {value}");

      Assert.AreEqual(value, data.UInt32ValueOption);
    }

    [TestMethod]
    public void Invalid_UInt32_With_Exceeding_Value_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments(@"-ui32 4294967296");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_UInt64_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);
      ulong value = 131072;

      target.ParseArguments($"-ui64 {value}");

      Assert.AreEqual(value, data.UInt64ValueOption);
    }

    [TestMethod]
    public void Invalid_UInt64_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var actual = target.ParseArguments(@"-ui64 -1");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Complex_String_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);

      string expected = "http://192.168.1.2/root-path/";

      data.StringValueOption = null;
      target.ParseArguments($@"-s ""{expected}""");
      Assert.AreEqual(expected, data.StringValueOption);

      data.StringValueOption = null;
      target.ParseArguments($@"-s:""{expected}""");
      Assert.AreEqual(expected, data.StringValueOption);

      data.StringValueOption = null;
      target.ParseArguments($@"/s ""{expected}""");
      Assert.AreEqual(expected, data.StringValueOption);

      data.StringValueOption = null;
      target.ParseArguments($@"/s:""{expected}""");
      Assert.AreEqual(expected, data.StringValueOption);
    }

    [TestMethod]
    public void Valid_Uri_Option_Shoud_Be_Parsed()
    {
      var data = new SimpleValidConfig();
      var target = new ConsoleCommandLineParser(data);
      var expected = "http://example.com/root/";
      var result = target.ParseArguments($@"/uri ""{expected}""");
      var actual = data.UriOptionValue.ToString();

      Assert.AreEqual(ResultStatus.Success, result.Status);
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Invalid_Uri_Option_Shoud_Be_Parsed_With_Error()
    {
      var target = new ConsoleCommandLineParser(new SimpleValidConfig());
      var result = target.ParseArguments($@"/uri ""C:\\Root\\Folder""");

      Assert.AreEqual(ResultStatus.Failure, result.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, result.Errors.First().ErrorType);
    }
  }
}
