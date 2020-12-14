using System;

namespace NArgs.Attributes
{
  /// <summary>
  /// Defines a command line parameter
  /// </summary>
  public class Parameter : Attribute
  {
    /// <summary>
    /// Gets or sets the ordinal number of a parameter
    /// </summary>
    public int OrdinalNumber
    {
      get
      {
        return _OrdinalNumber;
      }

      set
      {
        if (value < 1)
        {
          throw new ArgumentOutOfRangeException(nameof(this.OrdinalNumber));
        }

        _OrdinalNumber = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of a parameter
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the description of a parameter
    /// </summary>
    public string Description
    {
      get;
      set;
    }

    /// <summary>
    /// Creates a new instance of a command line parameter
    /// </summary>
    public Parameter()
    {
      this.OrdinalNumber = 1;
      this.Name = String.Empty;
      this.Description = String.Empty;
    }

    private int _OrdinalNumber;
  }
}
