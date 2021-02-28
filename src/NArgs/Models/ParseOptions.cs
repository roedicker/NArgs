using System;
using System.Collections.Generic;
using System.Globalization;

namespace NArgs.Models
{
  /// <summary>
  /// Defines the options used for parsing arguments
  /// </summary>
  public class ParseOptions
  {
    /// <summary>
    /// Gets or sets the culture used for parsing arguments
    /// </summary>
    public CultureInfo Culture
    {
      get;
      set;
    }

    /// <summary>
    /// Creates a new instance of the parse options
    /// </summary>
    public ParseOptions()
    {
      Culture = CultureInfo.CurrentUICulture;
    }
  }
}
