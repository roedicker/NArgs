using System;

namespace NArgs.Attributes
{
  /// <summary>
  /// Defines attribute for a command line option.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class OptionAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets the alternative name of an option.
    /// </summary>
    public string AlternativeName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the long name of an option.
    /// </summary>
    public string LongName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the usage-type for the display name of an option.
    /// </summary>
    public string UsageTypeDisplayName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets an indicator whether an option is required or not.
    /// </summary>
    public bool Required
    {
      get;
      set;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionAttribute" /> class.
    /// </summary>
    public OptionAttribute(): base()
    {
      AlternativeName = string.Empty;
      LongName = string.Empty;
      Required = false;
      UsageTypeDisplayName = "option";
    }
  }
}
