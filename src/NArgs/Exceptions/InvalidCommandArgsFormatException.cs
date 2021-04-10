using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NArgs
{
  /// <summary>
  /// Defines an exception regarding an invalid command argument format.
  /// </summary>
  [Serializable]
  public class InvalidCommandArgsFormatException : Exception
  {
    /// <summary>
    /// Gets the message of the exception.
    /// </summary>
    public override string Message
    {
      get
      {
        return _Message;
      }
    }

    /// <summary>
    /// Gets the name of the item parsed which caused the exception.
    /// </summary>
    public string ItemName
    {
      get;
      private set;
    }

    /// <summary>
    /// Instantiates a new instance of the <see cref="InvalidCommandArgsFormatException" /> class.
    /// </summary>
    internal InvalidCommandArgsFormatException() : base()
    {
      _Message = ExceptionMessage;
      ItemName = ItemDefaultName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCommandArgsFormatException" /> class with specified error message.
    /// </summary>
    /// <param name="message">Error message of exception.</param>
    public InvalidCommandArgsFormatException(string message) : base(message)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        _Message = ExceptionMessage;
      }
      else
      {
        _Message = $"{ExceptionMessage}. {message}";
      }

      ItemName = ItemDefaultName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCommandArgsFormatException" /> class with specified error messgae
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">Error message of exception.</param>
    /// <param name="innerException">Inner exception.</param>
    public InvalidCommandArgsFormatException(string message, Exception innerException) : base(message, innerException)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        _Message = ExceptionMessage;
      }
      else
      {
        _Message = $"{ExceptionMessage}. {message}";
      }

      ItemName = ItemDefaultName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCommandArgsFormatException" /> class with specified error message and optional item name.
    /// </summary>
    /// <param name="message">Error message of the exception.</param>
    /// <param name="itemName">Optional. Item name of the exception. Default value is <see langword="null" />.</param>
    public InvalidCommandArgsFormatException(string message, string? itemName = null) : base(message)
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
    /// Initializes a new instance of the <see cref="InvalidCommandArgsFormatException" /> class with given message, item name and inner exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="itemName">Item name of the exception</param>
    /// <param name="innerException">Inner exception</param>
    public InvalidCommandArgsFormatException(string message, string itemName, Exception innerException) : base(message, innerException)
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

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCommandArgsFormatException" /> class with serialized data.
    /// </summary>
    /// <param name="serializationInfo">Serialization information.</param>
    /// <param name="streamingContext">Streaming context.</param>
    protected InvalidCommandArgsFormatException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
      if (string.IsNullOrWhiteSpace(Message))
      {
        _Message = ExceptionMessage;
      }
      else
      {
        _Message = $"{ExceptionMessage}. {Message}";
      }


      ItemName = serializationInfo.GetString(nameof(ItemName));
    }

    /// <inheritdoc />
    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
      {
        throw new ArgumentNullException(nameof(info));
      }

      info.AddValue(nameof(ItemName), ItemName);
    }

    private const string ExceptionMessage = "Command-Args format is invalid";
    private const string ItemDefaultName = "n/a";

    private readonly string _Message;
  }
}
