using System;

namespace NArgs.Attributes
{
  /// <summary>
  /// Defines an NArgs attribute. 
  /// </summary>
  public abstract class Attribute : System.Attribute
  {
    /// <summary>
    /// Gets or sets the name of a NArgs attribute.
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the description of a NArgs attribute.
    /// </summary>
    public string Description
    {
      get;
      set;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Attribute" /> class.
    /// </summary>
    public Attribute()
    {
      Name = string.Empty;
      Description = string.Empty;
    }
  }
}
