using System;
using System.Collections.Generic;
using System.Linq;

namespace NArgs.Models
{
  /// <summary>
  /// Defines the command argument parse result.
  /// </summary>
  public class ParseResult
  {
    /// <summary>
    /// Gets the status of a command argument parse result.
    /// </summary>
    public ResultStatus Status
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets an indicator whether a result is a success or not.
    /// </summary>
    public bool Success => Status == ResultStatus.Success;


    /// <summary>
    /// Gets an indicator whether a result is a failure or not.
    /// </summary>
    public bool Failure => Status == ResultStatus.Failure;

    /// <summary>
    /// Gets a list of all errors of a command argument parse result.
    /// </summary>
    public IEnumerable<ParseError> Errors
    {
      get
      {
        return _Errors;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseResult" /> class.
    /// </summary>
    /// <param name="status">Optional. Status of the command argument parse result. Default value is <see cref="ResultStatus.Success"/>.</param>
    internal ParseResult(ResultStatus status = ResultStatus.Success)
    {
      _Errors = new List<ParseError>();

      Status = status;
    }

    /// <summary>
    /// Adds an error to the command argument parse result.
    /// </summary>
    /// <param name="error">Parse error to add.</param>
    internal void AddError(ParseError error)
    {
      if (error == null)
      {
        throw new ArgumentNullException(nameof(error));
      }

      Status = ResultStatus.Failure;
      _Errors.Add(error);
    }

    /// <summary>
    /// Adds errors to the command argument parse result.
    /// </summary>
    /// <param name="errors">Parse errors to add.</param>
    internal void AddErrors(IEnumerable<ParseError> errors)
    {
      if (errors == null)
      {
        throw new ArgumentNullException(nameof(errors));
      }

      if (errors.Any())
      {
        Status = ResultStatus.Failure;

        foreach (var error in errors)
        {
          _Errors.Add(error);
        }
      }
    }

    private readonly List<ParseError> _Errors;
  }
}
