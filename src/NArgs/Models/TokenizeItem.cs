using System;
using System.Collections.Generic;

namespace NArgs.Models
{
  /// <summary>
  /// Item representing a tokenized argument.
  /// </summary>
  public class TokenizeItem : IEquatable<TokenizeItem>
  {
    /// <summary>
    /// Gets or sets the name of a tokenized item.
    /// </summary>
    public string Name
    {
      get;
    }

    /// <summary>
    /// Gets or sets the value of a tokenized item.
    /// </summary>
    public string? Value
    {
      get;
      internal set;
    }

    /// <summary>
    /// Gets the result status of a tokenized item.
    /// </summary>
    public ResultStatus ResultStatus
    {
      get;
    }

    /// <summary>
    /// Gets the error type of a failed tokenized item.
    /// </summary>
    public TokenizeErrorType ErrorType
    {
      get;
    }

    /// <summary>
    /// Gets the error messgae of a failed tokenized item.
    /// </summary>
    public string ErrorMessage
    {
      get;
    }

    /// <summary>
    /// Gets an indicator wether tokizing of this item was successful or not.
    /// </summary>
    public bool Succeeded => ResultStatus == ResultStatus.Success;

    /// <summary>
    /// Gets an indicator wether tokizing of this item failed or not.
    /// </summary>
    public bool Failed => ResultStatus == ResultStatus.Failure;

    /// <summary>
    /// Initializes a new instance of a <see cref="TokenizeItem" /> class.
    /// </summary>
    public TokenizeItem()
    {
      Name = string.Empty;
      Value = null;
      ResultStatus = ResultStatus.Success;
      ErrorType = TokenizeErrorType.None;
      ErrorMessage = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="TokenizeItem" /> class with initial name and value.
    /// </summary>
    /// <param name="name">Name of the item.</param>
    /// <param name="value">Optional. Value of the item. Default value is <see langword="null" />.</param>
    public TokenizeItem(string name, string? value = null)
    {
      Name = name;
      Value = value;
      ResultStatus = ResultStatus.Success;
      ErrorType = TokenizeErrorType.None;
      ErrorMessage = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="TokenizeItem" /> class with initial name, error type and error message.
    /// </summary>
    /// <param name="name">Name of the item.</param>
    /// <param name="errorType">Type of error occured.</param>
    /// <param name="errorMessage">Message of error occured.</param>
    /// <remarks>This indicates a failed item.</remarks>
    public TokenizeItem(string name, TokenizeErrorType errorType, string? errorMessage)
    {
      Name = name;
      Value = null;
      ResultStatus = ResultStatus.Failure;
      ErrorType = errorType;
      ErrorMessage = errorMessage ?? string.Empty;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      return Equals(obj as TokenizeItem);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      int hashCode = 966633335;
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
      hashCode = hashCode * -1521134295 + EqualityComparer<string?>.Default.GetHashCode(Value);
      hashCode = hashCode * -1521134295 + ResultStatus.GetHashCode();
      hashCode = hashCode * -1521134295 + ErrorType.GetHashCode();
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ErrorMessage);
      return hashCode;
    }

    /// <summary>
    /// Determines whether the specific instance of class <see cref="TokenizeItem" /> is equal to the current instance.
    /// </summary>
    /// <param name="other">The instance to compare with the current instance.</param>
    /// <returns><see langword="true" /> if the specified instance is equal, otherwise <see langword="false" />.</returns>
    public virtual bool Equals(TokenizeItem? other)
    {
      return other == null ?
             false :
             Comparer.IsEqual(Name, other.Name) &&
             Comparer.IsEqual(Value, other.Value) &&
             ResultStatus == other.ResultStatus &&
             ErrorType == other.ErrorType &&
             Comparer.IsEqual(ErrorMessage, other.ErrorMessage);
    }
  }
}
