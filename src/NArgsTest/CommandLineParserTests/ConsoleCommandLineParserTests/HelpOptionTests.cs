using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NArgs;
using NArgsTest.CommandLineParserTests.Data;

namespace NArgsTest.CommandLineParserTests.ConsoleCommandLineParserTests
{
  [TestClass]
  public class HelpOptionTests
  {
    [TestMethod]
    public void HelpOption_Should_Raise_Proper_Event()
    {
      object command = null;
      var showUsage = false;
      var data = new TestHelpOptionConfiguration();
      var target = new ConsoleCommandLineParser(data);

      target.ExecuteUsage += (e) =>
      {
        showUsage = true;
        command = e.CommandName;
      };

      target.ParseArguments("c1 /R1 /?");

      Assert.AreEqual(true, showUsage, "Command has not been assigned via event as expected");
      Assert.IsNotNull(command, "Command has not been assigned via event as expected");
    }

    [TestMethod]
    public void HelpOption_Should_Show_UsageText_Based_On_Event()
    {
      var actual = string.Empty;
      var data = new TestHelpOptionConfiguration();
      var target = new ConsoleCommandLineParser(data);

      target.ExecuteUsage += (e) =>
      {
        actual = target.GetCommandUsageText("UnitTest", e.CommandName);
      };

      target.ParseArguments("c1 /R1 /?");

      var expected = @"c1: Example command #1

SYNTAX:
  UnitTest c1 <p1> [/s2] 

  PARAMETERS:
    p1     n/a

  OPTIONS:
    /s2 | --switch-2    
";

      Assert.AreEqual(expected, actual, "Command usage text is not as expected");
    }
  }
}
