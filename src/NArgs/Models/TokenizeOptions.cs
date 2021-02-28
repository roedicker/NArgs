using System;
using System.Collections.Generic;

namespace NArgs.Models
{
  /// <summary>
  /// Options of a command tokenizer.
  /// </summary>
  public class TokenizeOptions
  {
    /// <summary>
    /// Gets or sets the quotation character
    /// </summary>
    public char QuotationCharacter
    {
      get;
      set;
    }

    /// <summary>
    /// Gets a list of all tokenize separator characters.
    /// </summary>
    /// <returns>List of all tokenize separator characters</returns>
    public IEnumerable<char> Seperators
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets a list of all argument option name indicators
    /// </summary>
    /// <returns>List of all argument option name indicators</returns>
    public IEnumerable<string> ArgumentOptionNameIndicators
    {
      get;
      set;
    }

    /// <summary>
    /// Gets a list of all argument option value separator.
    /// </summary>
    /// <returns>List of all argument option value separators</returns>
    public IEnumerable<char> ArgumentOptionValueSeparators
    {
      get;
      set;
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="TokenizeOptions" /> class.
    /// </summary>
    public TokenizeOptions()
    {
      QuotationCharacter = QuotationDefaultCharacter;
      Seperators = new char[] { SeperatorDefaultCharacter, SeperatorAlternativeCharacter };
      ArgumentOptionNameIndicators = new string[] { ArgumentOptionDefaultNameIndicator, ArgumentOptionAlternativeNameIndicator, ArgumentOptionLongNameIndicator };
      ArgumentOptionValueSeparators = new char[] { ArgumentOptionDefaultValueSeparator, ArgumentOptionAlternativeValueSeparator };
    }

    private const char QuotationDefaultCharacter = '"';
    private const char SeperatorDefaultCharacter = ' ';
    private const char SeperatorAlternativeCharacter = (char)9;
    private const char ArgumentOptionAlternativeValueSeparator = '=';

    internal const string ArgumentOptionDefaultNameIndicator = "/";
    internal const string ArgumentOptionAlternativeNameIndicator = "-";
    internal const string ArgumentOptionLongNameIndicator = "--";
    internal const char ArgumentOptionDefaultValueSeparator = ':';
  }
}
