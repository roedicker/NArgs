using System;
using System.Collections.Generic;
using System.Linq;

namespace NArgs.Models
{
  /// <summary>
  /// Analysis result for given tokenized items and configuration
  /// </summary>
  internal class AnalysisResult
  {
    /// <summary>
    /// Gets the status of an analysis result.
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
    ///  Gets an indicator whether a configuration has commands defined or not.
    /// </summary>
    public bool HasCommandsDefined
    {
      get;
      internal set;
    }

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
    /// Initializes a new instance of the <see cref="AnalysisResult" /> class.
    /// </summary>
    internal AnalysisResult()
    {
      _Errors = new List<ParseError>();
      Status = ResultStatus.Success;
      HasCommandsDefined = false;
    }

    /// <summary>
    /// Adds an error to the analysis result.
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
    /// Adds errors to the analysis result.
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
        foreach (var error in errors)
        {
          _Errors.Add(error);
        }

        Status = ResultStatus.Failure;
      }
    }

    private readonly List<ParseError> _Errors;
  }
}
