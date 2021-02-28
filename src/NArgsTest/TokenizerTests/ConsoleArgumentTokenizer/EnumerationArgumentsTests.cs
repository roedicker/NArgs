using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NArgs.Models;
using NArgs.Services;

namespace NArgsTest.TokenizerTests
{
  [TestClass]
  public class EnumerationArgumentsTests
  {
    internal ConsoleArgumentTokenizer Target
    {
      get;
    }

    public EnumerationArgumentsTests()
    {
      Target = new ConsoleArgumentTokenizer();
    }

    [TestMethod]
    public void Option_With_No_Value_Should_Be_Tokenized()
    {
      var data = new string[] { "/o" };
      var expected = new List<TokenizeItem> { new TokenizeItem("/o") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }

    [TestMethod]
    public void Option_With_Value_Indicator_Should_Be_Tokenized()
    {
      var data = new string[] { "/option:value" };
      var expected = new List<TokenizeItem> { new TokenizeItem("/option", "value") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }

    [TestMethod]
    public void Option_With_Alternative_Value_Indicator_Should_Be_Tokenized()
    {
      var data = new string[] { "/option=value" };
      var expected = new List<TokenizeItem> { new TokenizeItem("/option", "value") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }

    [TestMethod]
    public void Option_With_Default_Indicator_And_Whitespaces_Should_Be_Tokenized()
    {
      var data = new string[] { "/option", " : ", "  value" };
      var expected = new List<TokenizeItem> { new TokenizeItem("/option", "value") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }

    [TestMethod]
    public void Option_With_Default_Indicator_Quoted_Value_Should_Be_Tokenized()
    {
      var data = new string[] { @"--date:""1970-04-01 10:43:28""" };
      var expected = new List<TokenizeItem> { new TokenizeItem("--date", "1970-04-01 10:43:28") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }

    [TestMethod]
    public void Option_With_Default_Indicator_And_Whitespaces_Quoted_Value_Should_Be_Tokenized()
    {
      var data = new string[] {"--date:", @"""1970-04-01 10:43:28"""};
      var expected = new List<TokenizeItem> { new TokenizeItem("--date", "1970-04-01 10:43:28") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }

    [TestMethod]
    public void Option_With_Alternative_Indicator_Quoted_Value_Should_Be_Tokenized()
    {
      var data = new string[] { "--date=", @"""1970-04-01 10:43:28""" };
      var expected = new List<TokenizeItem> { new TokenizeItem("--date", "1970-04-01 10:43:28") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }

    [TestMethod]
    public void Option_With_Alternative_Indicator_And_Whitespaces_Quoted_Value_Should_Be_Tokenized()
    {
      var data = new string[] { "--date", "=", @"""1970-04-01 10:43:28""" };
      var expected = new List<TokenizeItem> { new TokenizeItem("--date", "1970-04-01 10:43:28") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }

    [TestMethod]
    public void Option_With_Quoted_Value_Should_Be_Tokenized()
    {
      var data = new string[] { "--date", @"""1970-04-01 10:43:28""" };
      var expected = new List<TokenizeItem> { new TokenizeItem("--date"), new TokenizeItem(@"1970-04-01 10:43:28") };
      var actual = Target.Tokenize(data);

      Assert.IsTrue(expected.SequenceEqual(actual), "Expected tokenized items are not equal to the actual");
    }
  }
}
