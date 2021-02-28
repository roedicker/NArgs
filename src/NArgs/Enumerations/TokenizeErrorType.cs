using System;

namespace NArgs
{
  /// <summary>
  /// Error types for tokenizing arguments.
  /// </summary>
  public enum TokenizeErrorType
  {
    /// <summary>
    /// No error.
    /// </summary>
    None,

    /// <summary>
    /// Invalid character in name found.
    /// </summary>
    InvalidCharacterInName,

    /// <summary>
    /// Invalid character in value found.
    /// </summary>
    InvalidCharacterInValue,

    /// <summary>
    /// Quoted name or value is missing closing quotation.
    /// </summary>
    IncompleteQuotation,

    /// <summary>
    /// Unknown error.
    /// </summary>
    Unknown
  }
}
