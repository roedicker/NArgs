using System;

namespace NArgs.Enumerations
{
  /// <summary>
  /// Defines the current parse state.
  /// </summary>
  internal enum ParseState
  {
    /// <summary>
    /// Parser is currently processing an option or parameter.
    /// </summary>
    ScanOptionOrParameter,

    /// <summary>
    /// Parser is currently processing an option value.
    /// </summary>
    ScanOptionValue
  }
}
