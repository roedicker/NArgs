using System;

namespace NArgs.Exceptions
{
  /// <summary>
  /// Defines an exception regarding an invalid command argument format
  /// </summary>
  public class InvalidCommandArgsFormatException : Exception
  {
    /// <summary>
    /// Gets the message of the exception
    /// </summary>
    public override string Message
    {
      get
      {
        return _Message;
      }
    }

    /// <summary>
    /// Gets the name of the item parsed which caused the exception
    /// </summary>
    public string ItemName
    {
      get;
      private set;
    }

    /// <summary>
    /// Creates a new instance of this exception
    /// </summary>
    internal InvalidCommandArgsFormatException() : base()
    {
      _Message = ExceptionMessage;
      ItemName = ItemDefaultName;
    }

    /// <summary>
    /// Creates a new instance of this exception with given message and optional item name
    /// </summary>
    /// <param name="message">Message of the exception</param>
    /// <param name="itemName">Optional. Item name of the exception</param>
    internal InvalidCommandArgsFormatException(string message, string? itemName = null) : base(message)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        _Message = ExceptionMessage;
      }
      else
      {
        _Message = $"{ExceptionMessage}. {message}";
      }

      ItemName = string.IsNullOrWhiteSpace(itemName) ? ItemDefaultName : itemName ?? ItemDefaultName; // to avoid FX Cop warnings only
    }

    /// <summary>
    /// Creates a new instance of tis exception with given message, item name and inner exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="itemName">Item name of the exception</param>
    /// <param name="innerException">Inner exception</param>
    internal InvalidCommandArgsFormatException(string message, string itemName, Exception innerException) : base(message, innerException)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        _Message = ExceptionMessage;
      }
      else
      {
        _Message = $"{ExceptionMessage}. {message}";
      }

      if (string.IsNullOrWhiteSpace(itemName))
      {
        ItemName = ItemDefaultName;
      }
      else
      {
        ItemName = itemName;
      }
    }

    private const string ExceptionMessage = "Command-Args format is invalid";
    private const string ItemDefaultName = "n/a";

    private readonly string _Message;
  }
}
