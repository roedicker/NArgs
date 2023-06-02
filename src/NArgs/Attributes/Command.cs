using System;

namespace NArgs.Attributes;

/// <summary>
/// Defines the attribute for a command.
/// </summary>
public sealed class Command : Attribute
{
    /// <summary>
    /// Gets or sets the name of a command.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the long name of an option.
    /// </summary>
    public string LongName { get; set; }

    /// <summary>
    /// Gets or sets the description of a command.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Creates a new instance of a command.
    /// </summary>
    public Command()
    {
        Name = string.Empty;
        LongName = string.Empty;
        Description = string.Empty;
    }
}
