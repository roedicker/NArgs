using System;

namespace NArgs.Attributes;

/// <summary>
/// Defines a command line parameter.
/// </summary>
public sealed class Parameter : Attribute
{
    private int _ordinalNumber;

    /// <summary>
    /// Gets or sets the ordinal number of a parameter.
    /// </summary>
    public int OrdinalNumber
    {
        get
        {
            return _ordinalNumber;
        }

        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(OrdinalNumber));
            }

            _ordinalNumber = value;
        }
    }

    /// <summary>
    /// Gets or sets the name of a parameter.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of a parameter.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Creates a new instance of a command line parameter.
    /// </summary>
    public Parameter()
    {
        OrdinalNumber = 1;
        Name = string.Empty;
        Description = string.Empty;
    }
}
