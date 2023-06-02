using System;
using NArgs.Models;

namespace NArgs;

/// <summary>
/// Delegate for action based on found command.
/// </summary>
/// <param name="name">Command name.</param>
/// <param name="command">Command configuration.</param>
public delegate void CommandAction(string name, object command);

/// <summary>
/// Defines an argument parser.
/// </summary>
public interface IArgumentParser
{
    /// <summary>
    /// Gets the options of the argument parser.
    /// </summary>
    ParseOptions Options { get; }

    /// <summary>
    /// Registers a custom data-type handler.
    /// </summary>
    /// <param name="type">Type of the property to handle.</param>
    /// <param name="getter">Getter of the custom data-type.</param>
    /// <param name="validator">Validator of the custom data-type.</param>
    void RegisterCustomDataTypeHandler(Type type,
        PropertyServiceCustomDataTypeGetter getter,
        PropertyServiceCustomDataTypeValidator validator);

    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    /// <param name="config">Configuration to store parsed arguments.</param>
    /// <param name="args">Arguments as an array.</param>
    /// <returns>Parse result.</returns>
    ParseResult ParseArguments(object config, string[] args);

    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    /// <param name="config">Configuration to store parsed arguments.</param>
    /// <param name="args">Arguments as an array.</param>
    /// <param name="commandAction">Action to perform for a matching command.</param>
    /// <returns>Parse result.</returns>
    ParseResult ParseArguments(object config,
        string[] args,
        CommandAction commandAction);

    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    /// <param name="config">Configuration to store parsed arguments.</param>
    /// <param name="args">Arguments as a command line.</param>
    /// <returns>Parse result.</returns>
    ParseResult ParseArguments(object config, string args);

    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    /// <param name="config">Configuration to store parsed arguments.</param>
    /// <param name="args">Arguments as a command line.</param>
    /// <param name="commandAction">Action to perform for a matching command.</param>
    /// <returns>Parse result.</returns>
    ParseResult ParseArguments(object config,
        string args,
        CommandAction commandAction);

    /// <summary>
    /// Gets a formatted string based on the used configuration.
    /// </summary>
    /// <param name="config">Configuration to store parsed arguments.</param>
    /// <param name="executable">Name of the current executable used in formatted output.</param>
    /// <param name="commandName">Optional. Command name to get usage for.</param>
    /// <returns>Formatted output based on the configuration and current name of the executable and an optional command name.</returns>
    string GetUsage(object config,
        string executable,
        string commandName = null);
}
