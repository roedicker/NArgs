using System;

namespace NArgs.Enumerations
{
  /// <summary>
  /// Defines the current tokenize state.
  /// </summary>
  internal enum TokenizeState
  {
    /// <summary>
    /// Tokenizer is currently processing a name.
    /// </summary>
    ScanName,

    /// <summary>
    /// Tokenizer is currently processing the beginning of a value.
    /// </summary>
    ScanBeginValue,

    /// <summary>
    /// Tokenizer is currently processing a value.
    /// </summary>
    ScanValue,

    /// <summary>
    /// Tokenizer is currently processing a quoted name.
    /// </summary>
    ScanQuotedName,

    /// <summary>
    /// Tokenizer is currently processing a quoted value.
    /// </summary>
    ScanQuotedValue
  }
}
