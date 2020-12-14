using System;

namespace NArgs
{
  /// <summary>
  /// Defines the type of a parse error
  /// </summary>
  public enum ParseErrorType
  {
    /// <summary>
    /// Indicates that an unknown parse error has occurred
    /// </summary>
    UnknownError,

    /// <summary>
    /// Indicates that a parsing has detected an invalid command argument format
    /// </summary>
    InvalidCommandArgsFormat,

    /// <summary>
    /// Indicates that a parsing has detected an invalid option value
    /// </summary>
    /// 
    InvalidOptionValue,

    /// <summary>
    /// Indicates that a parsing has detected a missing required option value
    /// </summary>
    RequiredOptionValue
  }
}
