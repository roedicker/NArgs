using System;
using System.Collections.Generic;

using NArgs.Models;
using NArgs.Services;

namespace NArgs
{
  /// <summary>
  /// Defines the console command line argument parser.
  /// </summary>
  public class ConsoleCommandLineParser : CommandLineParser, IArgumentParser
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleCommandLineParser" /> class.
    /// </summary>
    /// <param name="configuration">Configuration attached to this parser.</param>
    public ConsoleCommandLineParser(object configuration) : base(new ConsoleArgumentTokenizer(), new DefaultPropertyService(configuration))
    {
      // not used
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleCommandLineParser" /> class with custom options.
    /// </summary>
    /// <param name="configuration">Configuration attached to this parser.</param>
    /// <param name="options">Commandline parser options to use.</param>
    public ConsoleCommandLineParser(object configuration, ParseOptions options) : base(new ConsoleArgumentTokenizer(), new DefaultPropertyService(configuration), options)
    {
      // not used
    }
  }
}
