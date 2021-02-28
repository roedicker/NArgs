using System;

namespace NArgs
{
  /// <summary>
  /// Defines constants for supported build-in data-types
  /// </summary>
  public static class PropertyTypeFullName
  {
    /// <summary>
    /// Defines the name for a system boolean
    /// </summary>
    public const string Boolean = "System.Boolean";

    /// <summary>
    /// Defines the name of a system character
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string Char = "System.Char";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system date-time
    /// </summary>
    public const string DateTime = "System.DateTime";

    /// <summary>
    /// Defines the name of a system directory-info
    /// </summary>
    public const string DirectoryInfo = "System.IO.DirectoryInfo";

    /// <summary>
    /// Defines the name of a system double
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string Double = "System.Double";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system file-info
    /// </summary>
    public const string FileInfo = "System.IO.FileInfo";

    /// <summary>
    /// Defines the name of a system 16-bit integer
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string Int16 = "System.Int16";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system 32-bit integer
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string Int32 = "System.Int32";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system 64-bit integer
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string Int64 = "System.Int64";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system single
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string Single = "System.Single";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system string
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string String = "System.String";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system unsigned 16-bit integer
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string UInt16 = "System.UInt16";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system unsigned 32-bit integer
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string UInt32 = "System.UInt32";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system unsigned 64-bit integer
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    public const string UInt64 = "System.UInt64";
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Defines the name of a system URI
    /// </summary>
    public const string Uri = "System.Uri";
  }
}
