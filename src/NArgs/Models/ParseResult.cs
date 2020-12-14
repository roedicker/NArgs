using System;
using System.Collections.Generic;

namespace NArgs.Models
{
  /// <summary>
  /// Defines the command argument parse result
  /// </summary>
  public class ParseResult
  {
    /// <summary>
    /// Gets the status of a command argument parse result
    /// </summary>
    public ResultStatus Status
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets a list of all errors of a command argument parse result
    /// </summary>
    public IEnumerable<ParseError> Errors
    {
      get
      {
        return _Errors;
      }
    }

    /// <summary>
    /// Creates a new instance of the command argument parse result
    /// </summary>
    /// <param name="status">Option. Status of the command argument parse result. Default value is <see cref="ResultStatus.Success"/>.</param>
    internal ParseResult(ResultStatus status = ResultStatus.Success)
    {
      _Errors = new List<ParseError>();

      this.Status = status;
    }

    /// <summary>
    /// Adds an error to the command argument parse result
    /// </summary>
    /// <param name="error">Parse error to add</param>
    internal void AddError(ParseError error)
    {
      if (error == null)
      {
        throw new ArgumentNullException(nameof(error));
      }

      this.Status = ResultStatus.Failure;
      _Errors.Add(error);
    }

    private readonly List<ParseError> _Errors;
  }
}
