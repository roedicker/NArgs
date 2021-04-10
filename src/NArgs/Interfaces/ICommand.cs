using System;

namespace NArgs
{
  /// <summary>
  /// Definition of a command line argument command.
  /// </summary>
  public interface ICommand
  {
    /// <summary>
    /// Executes the command when specified in the command arguments.
    /// </summary>
    void ExecuteCommand();
  }
}
