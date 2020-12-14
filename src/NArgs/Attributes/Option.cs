using System;

namespace NArgs.Attributes
{
  /// <summary>
  /// Defines attribute for a command line option
  /// </summary>
  public class Option : Attribute
  {
    /// <summary>
    /// Gets or sets the alternative name of an option
    /// </summary>
    public string AlternativeName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the name of an option
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the long name of an option
    /// </summary>
    public string LongName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the description of an option
    /// </summary>
    public string Description
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the usage-type for the display name of an option
    /// </summary>
    public string UsageTypeDisplayName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets an indicator whether an option is required or not
    /// </summary>
    public bool Required
    {
      get;
      set;
    }

    /// <summary>
    /// Creates a new instance of a command line option
    /// </summary>
    public Option()
    {
      this.AlternativeName = String.Empty;
      this.Name = String.Empty;
      this.LongName = String.Empty;
      this.Required = false;
      this.Description = String.Empty;
      this.UsageTypeDisplayName = "option";
    }
  }
}
