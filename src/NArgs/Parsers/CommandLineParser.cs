using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

using StringCollection = NCollections.StringCollection;

using NExtents;

using NArgs.Attributes;
using NArgs.Exceptions;
using NArgs.Models;

namespace NArgs
{
  /// <summary>
  /// Defines the base command line parser.
  /// </summary>
  public abstract class CommandLineParser : IArgumentParser
  {
    /// <summary>
    /// Gets or sets the property service.
    /// </summary>
    protected IPropertyService PropertyService
    {
      get;
    }

    /// <summary>
    /// Gets or sets the tokenizer.
    /// </summary>
    protected ICommandTokenizer Tokenizer
    {
      get;
    }

    /// <summary>
    /// Gets the argument parse options
    /// </summary>
    public ParseOptions Options
    {
      get;
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="CommandLineParser" /> class.
    /// </summary>
    /// <param name="tokenizer">Command argument tokenizer.</param>
    /// <param name="propertyService">Property service.</param>
    protected CommandLineParser(ICommandTokenizer tokenizer, IPropertyService propertyService)
    {
      Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
      PropertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
      Options = new ParseOptions();
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="CommandLineParser" /> class with custom options.
    /// </summary>
    /// <param name="tokenizer">Command argument tokenizer.</param>
    /// <param name="propertyService">Property service.</param>
    /// <param name="options">Commandline parser options to use.</param>
    protected CommandLineParser(ICommandTokenizer tokenizer, IPropertyService propertyService, ParseOptions options) : this(tokenizer, propertyService)
    {
      Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Parses the command line arguments.
    /// </summary>
    /// <returns>Parse result.</returns>
    virtual public ParseResult ParseArguments(string[] args)
    {
      return ParseItems(Tokenizer.Tokenize(args));
    }

    /// <inheritdoc />
    public ParseResult ParseArguments(string args)
    {
      return ParseItems(Tokenizer.Tokenize(args));
    }

    /// <summary>
    /// Gets the usage for the current configuration
    /// </summary>
    /// <param name="executable">Current executable name</param>
    /// <returns>Usage output for the current configuration</returns>
    public virtual string GetUsage(string executable)
    {
      executable ??= string.Empty;

      var result = new StringBuilder();
      var parameters = new List<Parameter>();
      using var usageSyntaxText = new StringCollection();
      using var optionsSyntaxText = new StringCollection();
      var options = new StringDictionary();
      var maxParameterNameLength = 0;
      var maxOptionCompleteNameLength = 0;

      foreach (var property in PropertyService.GetProperties())
      {
        if (property.GetCustomAttributes(typeof(Parameter), true).FirstOrDefault() is Parameter parameter)
        {
          parameters.Add(parameter);

          if (parameter.Name?.Length > maxParameterNameLength)
          {
            maxParameterNameLength = (int)parameter.Name.Length;
          }
        }
        else
        {
          if (property.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option oOption)
          {
            using var optionNames = new StringCollection();

            if (!string.IsNullOrWhiteSpace(oOption.Name))
            {
              optionNames.AddDistinct($"{TokenizeOptions.ArgumentOptionDefaultNameIndicator}{oOption.Name}");
            }

            if (!string.IsNullOrWhiteSpace(oOption.AlternativeName))
            {
              optionNames.AddDistinct($"{TokenizeOptions.ArgumentOptionDefaultNameIndicator}{oOption.AlternativeName}");
            }

            if (!string.IsNullOrWhiteSpace(oOption.LongName))
            {
              optionNames.AddDistinct($"{TokenizeOptions.ArgumentOptionLongNameIndicator}{oOption.LongName}");
            }

            var optionCompleteName = optionNames.ToString(" | ");

            if (oOption.Required)
            {
              if (property.PropertyType.FullName == PropertyTypeFullName.Boolean)
              {
                optionsSyntaxText.Add($"{optionCompleteName}");
              }
              else
              {
                optionsSyntaxText.Add($"{optionCompleteName} <{oOption.UsageTypeDisplayName}>");
              }
            }
            else
            {
              if (property.PropertyType.FullName == PropertyTypeFullName.Boolean)
              {
                optionsSyntaxText.Add($"[{optionCompleteName}]");
              }
              else
              {
                optionsSyntaxText.Add($"[{optionCompleteName} <{oOption.UsageTypeDisplayName}>]");
              }
            }

            options.Add(optionCompleteName, string.IsNullOrWhiteSpace(oOption.Description) ? Resources.NotApplicableValue : oOption.Description);

            if (optionCompleteName.Length > maxOptionCompleteNameLength)
            {
              maxOptionCompleteNameLength = optionCompleteName.Length;
            }
          }
        }
      }

      // sort parameters by their ordinal numbers
      parameters.Sort((l, r) => l.OrdinalNumber.CompareTo(r.OrdinalNumber));

      result.AppendLine($"{Resources.SyntaxCapitalizedName}:");
      result.Append($"  {executable} ");

      if (parameters.Count > 0)
      {
        foreach (var parameter in parameters)
        {
          result.AppendFormat(CultureInfo.InvariantCulture, "<{0}> ", string.IsNullOrWhiteSpace(parameter.Name) ? Resources.NotApplicableValue : parameter.Name);
        }
      }

      StringBuilder sbIndention = new StringBuilder();

      for (var i = 0; i < executable.Length + 3; i++)
      {
        sbIndention.Append(' ');
      }

      result.Append($"{optionsSyntaxText.ToString($"{Environment.NewLine}{sbIndention}")}{Environment.NewLine}");

      if (parameters.Count > 0)
      {
        result.AppendLine($"{Environment.NewLine}{Resources.ParametersCapitalizedName}:");

        foreach (var parameter in parameters)
        {
          result.AppendFormat(CultureInfo.InvariantCulture, "  {0, " + Convert.ToString(-maxParameterNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}", string.IsNullOrWhiteSpace(parameter.Name) ? "n/a" : parameter.Name, string.IsNullOrWhiteSpace(parameter.Description) ? "n/a" : parameter.Description, Environment.NewLine);
        }
      }

      if (options.Count > 0)
      {
        result.AppendLine($"{Environment.NewLine}{Resources.OptionsCapitalizedName}:");

        foreach (string key in options.Keys)
        {
          result.AppendFormat(CultureInfo.InvariantCulture, "  {0, " + Convert.ToString(-maxOptionCompleteNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}", key, options[key], Environment.NewLine);
        }
      }

      return result.ToString();
    }

    /// <inheritdoc />
    public void RegisterCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter setter, PropertyServiceCustomDataTypeValidator validator)
    {
      AddCustomDataTypeHandler(type, setter, validator);
    }

    private ParseResult ParseItems(IList<TokenizeItem> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException(nameof(items));
      }

      var result = new ParseResult();

      try
      {
        // check if tokenizing failed
        if (items.Any(item => item.Failed))
        {
          result.AddErrors(items.Where(item => item.Failed)
                                .Select(item => new ParseError(ParseErrorType.InvalidCommandArgsFormat,
                                                               item.Name,
                                                               item.Value,
                                                               item.ErrorMessage)));

          return result;
        }

        // TODO: Perform plausibility checks regarding identical option names and/or parameter ordinals

        uint ordinalNumber = 1;

        for (int i = 0; i < items.Count; i++)
        {
          var item = items[i];
          TokenizeItem? nextItem = (i + 1) < items.Count ? items[i + 1] : null;

          // perform plausibility checks
          if (string.IsNullOrWhiteSpace(item.Name))
          {
            result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat, item.Name, item.Value, "Required name is missing"));
          }

          // check if item refers to an option
          var property = DetermineOptionProperty(item);

          if (property != null)
          {
            // check if item contains directly a value or not
            if (string.IsNullOrEmpty(item.Value))
            {
              // special handling of boolean options: Either no value (treat as true) or optional following value
              if (property.PropertyType == typeof(bool))
              {
                // check if next item (if available) is a boolean value (use it) or not (ignore it)
                if (PropertyService.IsValidValue(property, nextItem?.Name))
                {
                  PropertyService.SetPropertyValue(property, nextItem?.Name);
                  i++;
                }
                else
                {
                  PropertyService.SetPropertyValue(property, true.ToLowerString());
                }
              }
              else
              {
                // option refers not to a boolean - validate value (next item)
                if (PropertyService.IsValidValue(property, nextItem?.Name))
                {
                  PropertyService.SetPropertyValue(property, nextItem?.Name);
                }
                else
                {
                  result.AddError(new ParseError(ParseErrorType.InvalidOptionValue,
                                                 GetOptionName(item.Name),
                                                 nextItem?.Name,
                                                 string.Format(CultureInfo.InvariantCulture,
                                                               Resources.ValueIsInvalidForOptionFormatErrorMessage,
                                                               nextItem?.Name,
                                                               GetOptionName(item.Name))));
                }

                i++;
              }
            }
            else
            {
              if (PropertyService.IsValidValue(property, item.Value))
              {
                PropertyService.SetPropertyValue(property, item.Value);
              }
              else
              {
                result.AddError(new ParseError(ParseErrorType.InvalidOptionValue,
                                               GetOptionName(item.Name),
                                               item.Value,
                                               string.Format(CultureInfo.InvariantCulture,
                                                             Resources.ValueIsInvalidForOptionFormatErrorMessage,
                                                             item.Value,
                                                             GetOptionName(item.Name))));
              }
            }
          }
          else
          {
            // check if item refers to a parameter
            property = DetermineParameterProperty(ordinalNumber);

            if (property == null)
            {
              // item neither refers to an option nor a parameter
              result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat, item.Name, item.Value, "Argument does not match any option or parameter"));
              continue;
            }
            else
            {
              if (PropertyService.IsValidValue(property, item.Name))
              {
                PropertyService.SetPropertyValue(property, item.Name);
              }
              else
              {
                result.AddError(new ParseError(ParseErrorType.InvalidParameterValue, property.Name, item.Name));
                continue;
              }

              ordinalNumber++;
            }
          }
        }

        // perform check for required values
        var plausibiltyResult = CheckRequiredProperties();

        if (plausibiltyResult.Status == ResultStatus.Failure)
        {
          result.AddErrors(plausibiltyResult.Errors);
        }
      }
      catch (InvalidCommandArgsFormatException ex)
      {
        result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat, ex.ItemName, null, ex.Message));
      }
#pragma warning disable CA1031 // Do not catch general exception types
      catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
      {
        result.AddError(new ParseError(ParseErrorType.UnknownError, Resources.NotAvailableShortName, null, ex.Message));
      }

      return result;
    }

    private PropertyInfo? DetermineOptionProperty(TokenizeItem item)
    {
      // check if current token is an option
      var itemName = item.Name.TrimStart(new[] { TokenizeOptions.ArgumentOptionLongNameIndicator,
                                                 TokenizeOptions.ArgumentOptionAlternativeNameIndicator,
                                                 TokenizeOptions.ArgumentOptionDefaultNameIndicator
                                               });

      return PropertyService.GetProperties()
                            .FirstOrDefault(p => p.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option &&
                                                 (option.Name == itemName ||
                                                  option.AlternativeName == itemName ||
                                                  option.LongName == itemName
                                                 ));
    }

    private ParseResult CheckRequiredProperties()
    {
      var result = new ParseResult();

      result.AddErrors(PropertyService.GetUnassignedRequiredOptionNames()
                                      .Select(name => new ParseError(ParseErrorType.RequiredOptionValue,
                                                                  name,
                                                                  Resources.NotApplicableValue,
                                                                  string.Format(CultureInfo.InvariantCulture,
                                                                                Resources.OptionIsMissingRequiredValueFormatErrorMessage,
                                                                                name))));

      return result;
    }

    private PropertyInfo? DetermineParameterProperty(uint ordinalNumber)
    {
      return PropertyService.GetProperties()
                                 .FirstOrDefault(p => p.GetCustomAttributes(typeof(Parameter), true).FirstOrDefault() is Parameter parameter && parameter.OrdinalNumber == ordinalNumber);
    }

    /// <summary>
    /// Adds a custom data-type handler
    /// </summary>
    /// <param name="type">Data-type to be handled</param>
    /// <param name="setter">Setter of the custom data-type</param>
    /// <param name="validator">Validator of the custom data-type</param>
    internal void AddCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter setter, PropertyServiceCustomDataTypeValidator validator)
    {
      PropertyService.AddCustomDataTypeHandler(type, setter, validator);
    }

    /// <summary>
    /// Gets the name of an option.
    /// </summary>
    /// <param name="itemName">Item name.</param>
    /// <returns>Option name of an item name.</returns>
    private string GetOptionName(string itemName)
    {
      return itemName?.Trim(Tokenizer.Options.ArgumentOptionNameIndicators) ?? string.Empty;
    }
  }
}
