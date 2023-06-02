using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NArgs
{
  /// <summary>
  /// Defines an exception regarding an invalid command argument format.
  /// </summary>
  [Serializable]
  public class InvalidConfigurationException : Exception
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
    /// Initializes a new instance of the <see cref="InvalidConfigurationException" /> class.
    /// </summary>
    internal InvalidConfigurationException() : base()
    {
      _Message = ExceptionMessage;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidConfigurationException" /> class with specified error message.
    /// </summary>
    /// <param name="message">Error message of exception.</param>
    public InvalidConfigurationException(string message) : base(message)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        _Message = ExceptionMessage;
      }
      else
      {
        _Message = $"{ExceptionMessage}. {message}";
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidConfigurationException" /> class with specified error message.
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">Error message of exception.</param>
    /// <param name="innerException">Inner exception.</param>
    public InvalidConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        _Message = ExceptionMessage;
      }
      else
      {
        _Message = $"{ExceptionMessage}. {message}";
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidConfigurationException" /> class with serialized data.
    /// </summary>
    /// <param name="serializationInfo">Serialization information.</param>
    /// <param name="streamingContext">Streaming context.</param>
    protected InvalidConfigurationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
      if (string.IsNullOrWhiteSpace(Message))
      {
        _Message = ExceptionMessage;
      }
      else
      {
        _Message = $"{ExceptionMessage}. {Message}";
      }
    }

    private const string ExceptionMessage = "Configuration is invalid";

    private readonly string _Message;
  }
}
