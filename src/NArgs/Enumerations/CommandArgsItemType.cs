using System;

namespace NArgs
{
  /// <summary>
  /// Defines the type of a command argument item
  /// </summary>
  public enum CommandArgsItemType: int
  {
    /// <summary>
    /// Indicates that no command argument item type has been set
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// Indicates that a command argument item is a parameter
    /// </summary>
    Parameter = 1,

    /// <summary>
    /// Indicates that a command argument item is an option
    /// </summary>
    Option = 2,

    /// <summary>
    /// Indicates that a command argument item is a long-named option
    /// </summary>
    OptionLongName = 3
  }
}
