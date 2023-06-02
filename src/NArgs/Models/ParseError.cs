using System;

namespace NArgs.Models;

/// <summary>
/// Defines a command argument parse error.
/// </summary>
public class ParseError
{
    /// <summary>
    /// Gets the error-type of a parse error.
    /// </summary>
    public ParseErrorType ErrorType { get; }

    /// <summary>
    /// Gets the item name of a parse error.
    /// </summary>
    public string ItemName { get; }

    /// <summary>
    /// Gets the item value of a parse error.
    /// </summary>
    public string ItemValue { get; }

    /// <summary>
    /// Gets the message of a parse error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Creates a new instance of a parse error.
    /// </summary>
    /// <param name="errorType">Error type of the parse error.</param>
    /// <param name="itemName">Item name of the parse error.</param>
    /// <param name="itemValue">Item value of the parse error.</param>
    /// <param name="message">Optional. Message of the pare error.</param>
    internal ParseError(ParseErrorType errorType,
        string itemName,
        string itemValue,
        string message = null)
    {
        if (string.IsNullOrWhiteSpace(itemName))
        {
            throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(itemName));
        }

        ErrorType = errorType;
        ItemName = itemName;
        ItemValue = itemValue;
        Message = message ?? string.Empty;
    }
}
