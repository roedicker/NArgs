using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NArgs;
using NArgsTest.CommandLineParserTests.Data;

namespace NArgsTest.CommandLineParserTests.ConsoleCommandLineParserTests
{
  [TestClass]
  public class CommandTests
  {
    [TestMethod]
    public void GetUsageText_Should_Be_As_Expected()
    {
      var target = new ConsoleCommandLineParser(new TestCommandConfiguration());
      var expected = @"SYNTAX:
  UnitTest  <command> [/s1] 

  COMMANDS:
    c1     Example command #1

  OPTIONS:
    /s1 | --switch-1    
";

      var actual = target.GetUsageText("UnitTest");

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetCommandUsageText_Should_Be_As_Expected()
    {
      var data = new TestCommandConfiguration();
      var target = new ConsoleCommandLineParser(data);
      var expected = @"c1: Example command #1

SYNTAX:
  UnitTest c1 <p1> [/s2] 

  PARAMETERS:
    p1     n/a

  OPTIONS:
    /s2 | --switch-2    
";

      var actual = target.GetCommandUsageText("UnitTest", "c1");

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Command_Should_Raise_Proper_Event()
    {
      object command = null;
      var data = new TestCommandConfiguration();
      var target = new ConsoleCommandLineParser(data);
      target.ExecuteCommand += (e) => command = e.CommandTarget;

      target.ParseArguments("c1 /s1");

      Assert.IsNotNull(command, "Command has not been assigned via event as expected");
      Assert.AreSame(command, data.Command1, "Command raised not the event as expected");
    }

    [TestMethod]
    public void Command_Option_Should_Be_Set()
    {
      var data = new TestCommandConfiguration();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("c1 /s2 value");

      Assert.AreEqual(true, data.Command1.Switch2, "Command option has not been set as expected");
    }

    [TestMethod]
    public void Command_Parameter_Should_Be_Set()
    {
      var data = new TestCommandConfiguration();
      var target = new ConsoleCommandLineParser(data);

      target.ParseArguments("c1 value");

      Assert.AreEqual("value", data.Command1.Name1, "Command parameter has not been set as expected");
      Assert.AreEqual(true, data.Command1.HasCommandBeenExecuted, "Command execution indicator has not been set as expected");
    }
  }
}
