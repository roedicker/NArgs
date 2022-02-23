using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NArgs;
using NArgs.Extensions;
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
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, "--help -v:yes");

      Assert.AreEqual(true, oConfig.ShowHelpOption);
      Assert.AreEqual(true, oConfig.VerboseMessagesOption);
    }

    [TestMethod]
    public void Valid_Bool_Option_With_AlternativeName_Should_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, "/?");

      Assert.AreEqual(true, oConfig.ShowHelpOption);
    }

    [TestMethod]
    public void Valid_Bool_Option_With_AlternativeName_And_Value_Should_be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, "/?:yes");

      Assert.AreEqual(true, oConfig.ShowHelpOption);
    }

    [TestMethod]
    public void Valid_Bool_Option_With_AlternativeName_And_Alternative_Value_Should_be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, "/? false");

      Assert.AreEqual(false, oConfig.ShowHelpOption);
    }

    [TestMethod]
    public void Invalid_Bool_Option_Should_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, "/h Nein");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
      Assert.AreEqual($@"Value ""Nein"" is invalid for option ""h""", actual.Errors.First().Message);
    }

    [TestMethod]
    public void Invalid_Bool_Option_With_LongName_Should_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, "--help Nein");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
      Assert.AreEqual($@"Value ""Nein"" is invalid for option ""help""", actual.Errors.First().Message);
    }

    [TestMethod]
    public void Invalid_Bool_Option_With_AlternativeName_Should_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, "/? Nein");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
      Assert.AreEqual($@"Value ""Nein"" is invalid for option ""?""", actual.Errors.First().Message);
    }

    [TestMethod]
    public void Valid_Char_Option_With__Lowercase_Value_Should_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, "-c a");

      Assert.AreEqual('a', oConfig.CharValueOption);
    }

    [TestMethod]
    public void Valid_Char_Option_With_Uppercase_Value_Should_be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, "-c A");

      Assert.AreEqual('A', oConfig.CharValueOption);
    }

    [TestMethod]
    public void Valid_Char_Option_With_Single_Numeric_Value_Should_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, "-c 1");

      Assert.AreEqual('1', oConfig.CharValueOption);
    }

    [TestMethod]
    public void Invalid_Char_Option_With_String_Value_Should_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, "-c abc");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Invalid_Char_Option_With_Long_Numeric_Value_Should_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, "-c 123");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_String_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-s:""Example Name""");

      Assert.AreEqual("Example Name", oConfig.StringValueOption);
    }

    [TestMethod]
    public void Valid_Int16_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-i16 32767");

      Assert.AreEqual(32767, oConfig.Int16ValueOption);
    }

    [TestMethod]
    public void Invalid_Int16_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, @"-i16 32768");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_Int32_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-i 123");

      Assert.AreEqual(123, oConfig.Int32ValueOption);
    }

    [TestMethod]
    public void Invalid_Int32_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, @"-i 1.0");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_Int64_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-l 1234567890");

      Assert.AreEqual(1234567890, oConfig.Int64ValueOption);
    }

    [TestMethod]
    public void Invalid_Int64_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, @"-l abc");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_DateTime_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"--date ""1970-04-01 10:43:28""");

      Assert.AreEqual(DateTime.Parse("1970-04-01 10:43:28"), oConfig.DateValueOption);
    }

    [TestMethod]
    public void Valid_Double_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      // set / restore current thread's culture due to culture-depending notation
      CultureInfo oCurrentCulture = Thread.CurrentThread.CurrentCulture;

      try
      {
        oParser.ParseArguments(oConfig, $@"--double {3.14159265358979}");

        Assert.AreEqual((double)3.14159265358979, oConfig.DoubleValueOption);
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = oCurrentCulture;
      }
    }

    [TestMethod]
    public void Valid_SecureString_Option_Should_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      string value = "this is my secret";
      oParser.ParseArguments(oConfig, $@"-secure-string ""{value}""");

      Assert.AreEqual(value, oConfig.SecureStringOption.GetString());
    }

    [TestMethod]
    public void Invalid_Double_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, $@"--double a");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_FileInfo_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-file ""C:\Windows\explorer.exe""");

      Assert.IsNotNull(oConfig.FileInfoValueOption);
    }

    [TestMethod]
    public void Valid_Directory_Info_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-dir ""C:\Windows""");

      Assert.IsNotNull(oConfig.DirInfoValueOption);
    }

    [TestMethod]
    public void Valid_Parameters_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"""First Parameter Value"" ""Second Parameter Value""");

      Assert.AreEqual("First Parameter Value", oConfig.TextValueParameter1);
      Assert.AreEqual("Second Parameter Value", oConfig.TextValueParameter2);
    }

    [TestMethod]
    public void Valid_Mixed_Arguments_With_Correct_Sequence_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"""First Parameter Value"" ""Second Parameter Value"" --verbose:yes -i:123");

      Assert.AreEqual("First Parameter Value", oConfig.TextValueParameter1);
      Assert.AreEqual("Second Parameter Value", oConfig.TextValueParameter2);
      Assert.AreEqual(true, oConfig.VerboseMessagesOption);
      Assert.AreEqual(123, oConfig.Int32ValueOption);
    }

    [TestMethod]
    public void Invalid_Mixed_Arguments_With_Incorrect_Sequence_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
      ParseResult actual;
      ParseError oActualError;

      actual = oParser.ParseArguments(oConfig, @"--verbose ""First Parameter Value"" ""Second Parameter Value"" -i 123 ");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(2, actual.Errors.Count());

      oActualError = actual.Errors.First();
      Assert.AreEqual(ParseErrorType.InvalidCommandArgsFormat, oActualError.ErrorType);
      Assert.AreEqual("Second Parameter Value", oActualError.ItemName);

      oActualError = actual.Errors.Last();
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, oActualError.ErrorType);
      Assert.AreEqual("verbose", oActualError.ItemName);
      Assert.AreEqual("First Parameter Value", oActualError.ItemValue);
    }

    [TestMethod]
    public void Invalid_Mixed_Arguments_With_Alternative_Incorrect_Sequence_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
      ParseResult actual;
      ParseError oActualError;

      actual = oParser.ParseArguments(oConfig, @"""First Parameter Value"" /i:123 --verbose ""Second Parameter Value""");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(1, actual.Errors.Count());

      oActualError = actual.Errors.First();
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, oActualError.ErrorType);
      Assert.AreEqual("verbose", oActualError.ItemName);
      Assert.AreEqual("Second Parameter Value", oActualError.ItemValue);
    }

    [TestMethod]
    public void Invalid_FileInfo_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
      ParseResult actual;

      actual = oParser.ParseArguments(oConfig, @"-file ""?;""");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(1, actual.Errors.Count());

      ParseError oActualError = actual.Errors.First();

      Assert.AreEqual(ParseErrorType.InvalidOptionValue, oActualError.ErrorType);
      Assert.AreEqual("file", oActualError.ItemName);
      Assert.AreEqual("?;", oActualError.ItemValue);
    }

    [TestMethod]
    public void Valid_Custom_DataType_Option_Shoud_Be_Parsed()
    {
      CustomDataTypeConfig oConfig = new CustomDataTypeConfig();

      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
      oParser.RegisterCustomDataTypeHandler(typeof(Color), (name, value) =>
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

      oParser.ParseArguments(oConfig, @"-color ""Green""");

      Assert.AreEqual(Color.Green, oConfig.Color);
    }

    [TestMethod]
    public void Valid_Single_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      // set / restore current thread's culture due to culture-depending notation
      CultureInfo oCurrentCulture = Thread.CurrentThread.CurrentCulture;

      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        oParser.ParseArguments(oConfig, $@"--single {3.402823E+38}");

        Assert.AreEqual((float)3.402823E+38, oConfig.SingleValueOption);
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = oCurrentCulture;
      }
    }

    [TestMethod]
    public void Invalid_Single_Option_With_Exceeding_Value_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
      ParseResult actual = oParser.ParseArguments(oConfig, $@"--single {3.402823E+39}");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Parse_Valid_UInt16_Long_Option()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-ui16 65535");

      Assert.AreEqual((ushort)65535, oConfig.UInt16ValueOption);
    }

    [TestMethod]
    public void Invalid_UInt16_With_Exceeding_Value_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, @"-ui16 65536");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_UInt32_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-ui32 65536");

      Assert.AreEqual((uint)65536, oConfig.UInt32ValueOption);
    }

    [TestMethod]
    public void Invalid_UInt32_With_Exceeding_Value_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, @"-ui32 4294967296");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Valid_UInt64_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      oParser.ParseArguments(oConfig, @"-ui64 131072");

      Assert.AreEqual((ulong)131072, oConfig.UInt64ValueOption);
    }

    [TestMethod]
    public void Invalid_UInt64_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult actual = oParser.ParseArguments(oConfig, @"-ui64 -1");

      Assert.AreEqual(ResultStatus.Failure, actual.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, actual.Errors.First().ErrorType);
    }

    [TestMethod]
    public void Complex_String_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      string expected = "http://192.168.1.2/root-path/";

      oConfig.StringValueOption = null;
      oParser.ParseArguments(oConfig, $@"-s ""{expected}""");
      Assert.AreEqual(expected, oConfig.StringValueOption);

      oConfig.StringValueOption = null;
      oParser.ParseArguments(oConfig, $@"-s:""{expected}""");
      Assert.AreEqual(expected, oConfig.StringValueOption);

      oConfig.StringValueOption = null;
      oParser.ParseArguments(oConfig, $@"/s ""{expected}""");
      Assert.AreEqual(expected, oConfig.StringValueOption);

      oConfig.StringValueOption = null;
      oParser.ParseArguments(oConfig, $@"/s:""{expected}""");
      Assert.AreEqual(expected, oConfig.StringValueOption);
    }

    [TestMethod]
    public void Valid_Uri_Option_Shoud_Be_Parsed()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      string expected = "http://example.com/root/";
      ParseResult oResult = oParser.ParseArguments(oConfig, $@"/uri ""{expected}""");

      Assert.AreEqual(ResultStatus.Success, oResult.Status);
      Assert.AreEqual(expected, oConfig.UriOptionValue.ToString());
    }

    [TestMethod]
    public void Invalid_Uri_Option_Shoud_Be_Parsed_With_Error()
    {
      SimpleValidConfig oConfig = new SimpleValidConfig();
      ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();

      ParseResult oResult = oParser.ParseArguments(oConfig, $@"/uri ""C:\\Root\\Folder""");

      Assert.AreEqual(ResultStatus.Failure, oResult.Status);
      Assert.AreEqual(ParseErrorType.InvalidOptionValue, oResult.Errors.First().ErrorType);
    }
  }
}
