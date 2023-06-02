using System.Collections.Generic;
using System.Globalization;

namespace NArgs.Models;

/// <summary>
/// Defines the options used for parsing arguments.
/// </summary>
public sealed class ParseOptions
{
    private const string _commandArgsOptionDefaultNameIndicator = "/";
    private const string _commandArgsOptionAlternativeNameIndicator = "-";
    private const string _commandArgsOptionLongNameIndicator = "--";
    private const string _commandArgsOptionValueIndicator = ":";
    private const char _commandArgsQuotationCharacter = '"';
    private const char _commandArgsDefaultSeparatorCharacter = ' ';

    /// <summary>
    /// Gets or sets the culture used for parsing arguments.
    /// </summary>
    public CultureInfo Culture { get; set; }

    /// <summary>
    /// Gets or sets the default argument option name indicator. Default value is &quot;/&quot;.
    /// </summary>
    public string ArgumentOptionDefaultNameIndicator { get; set; }

    /// <summary>
    /// Gets or sets the alternative argument option name indicator. Default value is &quot;-&quot;.
    /// </summary>
    public string ArgumentOptionAlternativeNameIndicator { get; set; }

    /// <summary>
    /// Gets or sets the long-name argument option indicator. Default value is &quot;--&quot;.
    /// </summary>
    public string ArgumentOptionLongNameIndicator { get; set; }

    /// <summary>
    /// Gets or sets the argument option value indicator. Default value is&quot;:&quot;.
    /// </summary>
    public string ArgumentOptionValueIndicator { get; set; }

    /// <summary>
    /// Gets or sets the argument quotation character. Default value is '&quot;'.
    /// </summary>
    public char ArgumentQuotationCharacter { get; set; }

    /// <summary>
    /// Gets or sets the default argument separator character. Default value is ' '.
    /// </summary>
    /// <remarks>The following separators are used by default: Tabulator and Space. This property can be used to define an additional separator.</remarks>
    public char ArgumentDefaultSeparatorCharacter { get; set; }

    /// <summary>
    /// Gets a list of all argument option name indicators
    /// </summary>
    /// <returns>List of all argument option name indicators.</returns>
    public IEnumerable<string> GetArgumentOptionNameIndicators()
    {
        return new string[]
        {
            ArgumentOptionDefaultNameIndicator,
            ArgumentOptionAlternativeNameIndicator,
            ArgumentOptionLongNameIndicator
        };
    }

    /// <summary>
    /// Creates a new instance of the parse options.
    /// </summary>
    public ParseOptions()
    {
        Culture = CultureInfo.CurrentUICulture;
        ArgumentOptionDefaultNameIndicator = _commandArgsOptionDefaultNameIndicator;
        ArgumentOptionAlternativeNameIndicator = _commandArgsOptionAlternativeNameIndicator;
        ArgumentOptionLongNameIndicator = _commandArgsOptionLongNameIndicator;
        ArgumentOptionValueIndicator = _commandArgsOptionValueIndicator;
        ArgumentQuotationCharacter = _commandArgsQuotationCharacter;
        ArgumentDefaultSeparatorCharacter = _commandArgsDefaultSeparatorCharacter;
    }
}
