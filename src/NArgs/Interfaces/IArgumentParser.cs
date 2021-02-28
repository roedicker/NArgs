using System;

using NArgs.Models;

namespace NArgs
{
  /// <summary>
  /// Defines an argument parse
  /// </summary>
  public interface IArgumentParser
  {
    /// <summary>
    /// Registers a custom data-type handler
    /// </summary>
    /// <param name="type">Typoe of the property to handle</param>
    /// <param name="setter">Setter of the custom data-type</param>
    /// <param name="validator">Validator of the custom data-type</param>
    void RegisterCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter setter, PropertyServiceCustomDataTypeValidator validator);

    /// <summary>
    /// Parses command line arguments
    /// </summary>
    /// <param name="args">Arguments as an array</param>
    /// <returns>Parse result</returns>
    ParseResult ParseArguments(string[] args);

    /// <summary>
    /// Parses command line arguments
    /// </summary>
    /// <param name="args">Arguments as a command line</param>
    /// <returns>Parse result</returns>
    ParseResult ParseArguments(string args);

    /// <summary>
    /// Gets a formatted string based on the used configuration
    /// </summary>
    /// <param name="executable">Name of the current executable used in formatted output</param>
    /// <returns>Formatted output based on the configuration and current name of the executable</returns>
    string GetUsage(string executable);
  }
}
