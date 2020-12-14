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
    /// Gets or sets the default argument option name indicator. Default value is &quot;/&quot;.
    /// </summary>
    public string ArgumentOptionDefaultNameIndicator
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the alternative argument option name indicator. Default value is &quot;-&quot;.
    /// </summary>
    public string ArgumentOptionAlternativeNameIndicator
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the long-name argument option indicator. Default value is &quot;--&quot;.
    /// </summary>
    public string ArgumentOptionLongNameIndicator
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the argument option value indicator. Default value is&quot;:&quot;
    /// </summary>
    public string ArgumentOptionValueIndicator
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the argument quotation character. Default value is '&quot;'.
    /// </summary>
    public char ArgumentQuotationCharacter
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the default argument separator character. Default value is ' '.
    /// </summary>
    /// <remarks>The following separators are used by default: Tabulator and Space. This property can be used to define an additional separator.</remarks>
    public char ArgumentDefaultSeparatorCharacter
    {
      get;
      set;
    }

    /// <summary>
    /// Gets a list of all argument option name indicators
    /// </summary>
    /// <returns>List of all argument option name indicators</returns>
    public IEnumerable<string> GetArgumentOptionNameIndicators()
    {
      return new string[] { this.ArgumentOptionDefaultNameIndicator, this.ArgumentOptionAlternativeNameIndicator, this.ArgumentOptionLongNameIndicator };
    }

    /// <summary>
    /// Creates a new instance of the parse options
    /// </summary>
    public ParseOptions()
    {
      this.Culture = CultureInfo.CurrentUICulture;
      this.ArgumentOptionDefaultNameIndicator = CommandArgsOptionDefaultNameIndicator;
      this.ArgumentOptionAlternativeNameIndicator = CommandArgsOptionAlternativeNameIndicator;
      this.ArgumentOptionLongNameIndicator = CommandArgsOptionLongNameIndicator;
      this.ArgumentOptionValueIndicator = CommandArgsOptionValueIndicator;
      this.ArgumentQuotationCharacter = CommandArgsQuotationCharacter;
      this.ArgumentDefaultSeparatorCharacter = CommandArgsDefaultSeperatorCharacter;
    }

    private const string CommandArgsOptionDefaultNameIndicator = "/";
    private const string CommandArgsOptionAlternativeNameIndicator = "-";
    private const string CommandArgsOptionLongNameIndicator = "--";
    private const string CommandArgsOptionValueIndicator = ":";
    private const char CommandArgsQuotationCharacter = '"';
    private const char CommandArgsDefaultSeperatorCharacter = ' ';
  }
}
