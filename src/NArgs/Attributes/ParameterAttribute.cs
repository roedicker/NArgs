using System;

namespace NArgs.Attributes
{
  /// <summary>
  /// Defines a command line parameter.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class ParameterAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets the ordinal number of a parameter.
    /// </summary>
    public uint OrdinalNumber
    {
      get
      {
        return _OrdinalNumber;
      }

      set
      {
        if (value < 1)
        {
          throw new ArgumentOutOfRangeException(nameof(OrdinalNumber));
        }

        _OrdinalNumber = value;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterAttribute" /> class.
    /// </summary>
    public ParameterAttribute() : base()
    {
      OrdinalNumber = 1;
    }

    private uint _OrdinalNumber;
  }
}
