using System;

using NArgs.Models;

namespace NArgs
{
  /// <summary>
  /// Handler vor performing an execute command event.
  /// </summary>
  /// <param name="e">Command event arguments.</param>
  public delegate void ExecuteCommandHandler(CommandEventArgs e);

  /// <summary>
  /// Defines an argument parser.
  /// </summary>
  public interface IArgumentParser
  {
    /// <summary>
    /// Execution event for a selected command.
    /// </summary>
    event ExecuteCommandHandler? ExecuteCommand;

    /// <summary>
    /// Registers a custom data-type handler.
    /// </summary>
    /// <param name="type">Typoe of the property to handle.</param>
    /// <param name="setter">Setter of the custom data-type.</param>
    /// <param name="validator">Validator of the custom data-type.</param>
    void RegisterCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter setter, PropertyServiceCustomDataTypeValidator validator);

    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    /// <param name="args">Arguments as an array.</param>
    /// <returns>Parse result.</returns>
    ParseResult ParseArguments(string[] args);

    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    /// <param name="args">Arguments as a command line.</param>
    /// <returns>Parse result.</returns>
    ParseResult ParseArguments(string args);

    /// <summary>
    /// Gets a formatted usage-text based on the used configuration.
    /// </summary>
    /// <param name="executableName">Name of the current executable used in formatted output.</param>
    /// <returns>Formatted usage-text based on the configuration and current name of the executable.</returns>
    string GetUsageText(string executableName);

    /// <summary>
    /// Gets a formatted usage-text based on a given command object.
    /// </summary>
    /// <param name="executableName">Name of the current executable used in formatted output.</param>
    /// <param name="commandName">Name of the command.</param>
    /// <returns>Formatted usage-text based on the given parameter.</returns>
    /// <exception cref="ArgumentException">Name is either not provided or not a valid command.</exception>
    string GetCommandUsageText(string executableName, string commandName);
  }
}
