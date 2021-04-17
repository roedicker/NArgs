using System;

namespace NArgs.Models
{
  /// <summary>
  /// Arguments of a usage event.
  /// </summary>
  public sealed class UsageEventArgs
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public string? CommandName
    {
      get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsageEventArgs" /> class.
    /// </summary>
    public UsageEventArgs()
    {
      CommandName = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsageEventArgs" /> class with command name.
    /// </summary>
    /// <param name="commandName">Command name.</param>
    public UsageEventArgs(string commandName)
    {
      if (string.IsNullOrWhiteSpace(commandName))
      {
        throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(commandName));
      }

      CommandName = commandName;
    }
  }
}
