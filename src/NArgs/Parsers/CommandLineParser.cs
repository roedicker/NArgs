using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

using NExtents;

using NArgs.Attributes;
using NArgs.Models;

namespace NArgs
{
  /// <summary>
  /// Defines the base command line parser.
  /// </summary>
  public abstract class CommandLineParser : IArgumentParser
  {
    /// <inheritdoc />
    public event ExecuteCommandHandler? ExecuteCommand;

    /// <inheritdoc />
    public event ExecuteUsageHandler? ExecuteUsage;

    /// <summary>
    /// Gets the property service.
    /// </summary>
    protected IPropertyService PropertyService
    {
      get;
    }

    /// <summary>
    /// Gets or sets the command property service.
    /// </summary>
    protected IPropertyService? CommandPropertyService
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the tokenizer.
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
    /// Initializes a new instance of the <see cref="CommandLineParser" /> class.
    /// </summary>
    /// <param name="tokenizer">Command argument tokenizer.</param>
    /// <param name="propertyService">Property service.</param>
    protected CommandLineParser(ICommandTokenizer tokenizer, IPropertyService propertyService)
    {
      Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
      PropertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
      CommandPropertyService = null;
      Options = new ParseOptions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineParser" /> class with custom options.
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

    /// <inheritdoc />
    public virtual string GetUsageText(string executableName)
    {
      executableName ??= string.Empty;

      var result = new StringBuilder();

      var parameterInfo = GetParameterUsageInfo();
      var optionInfo = GetOptionUsageInfo();
      var commandInfo = GetCommandUsageInfo();
      var indention = executableName.Length + 3;

      // build syntax text
      result.AppendLine($"{Resources.SyntaxCapitalizedName}:");
      result.Append($"  {executableName} ");

      result.Append(commandInfo.GetUsageSyntaxText(indention));
      result.Append(parameterInfo.GetUsageSyntaxText(indention));
      result.Append(optionInfo.GetUsageSyntaxText(indention));

      var indentionBuilder = new StringBuilder();

      for (var i = 0; i < executableName.Length + 3; i++)
      {
        indentionBuilder.Append(' ');
      }

      // build detail text
      result.Append($"{Environment.NewLine}");

      result.Append(commandInfo.GetUsageDetailText());
      result.Append(parameterInfo.GetUsageDetailText());
      result.Append(optionInfo.GetUsageDetailText());

      return result.ToString();
    }

    /// <inheritdoc />
    public string GetCommandUsageText(string executableName, string commandName)
    {
      if (string.IsNullOrWhiteSpace(commandName))
      {
        throw new ArgumentNullException(Resources.MissingRequiredParameterValueErrorMessage, nameof(commandName));
      }

      executableName ??= string.Empty;

      var prop = PropertyService.GetPropertyByCommandName(commandName);
      var command = PropertyService.GetCommand(prop);
      var commandObject = PropertyService.GetGlobalPropertyValue(prop);
      var result = new StringBuilder();

      PropertyService.SetCurrentCommand(commandObject);

      try
      {
        var parameterInfo = GetParameterUsageInfo();
        var optionInfo = GetOptionUsageInfo();
        var commandInfo = GetCommandUsageInfo();
        var indention = executableName.Length + 3;

        // build description text
        result.AppendLine($"{command.Name}: {command.Description}{Environment.NewLine}");

        // build syntax text
        result.AppendLine($"{Resources.SyntaxCapitalizedName}:");
        result.Append($"  {executableName} {commandName} ");

        result.Append(parameterInfo.GetUsageSyntaxText(indention));
        result.Append(optionInfo.GetUsageSyntaxText(indention));

        var indentionBuilder = new StringBuilder();

        for (var i = 0; i < executableName.Length + 3; i++)
        {
          indentionBuilder.Append(' ');
        }

        // build detail text
        result.Append($"{Environment.NewLine}");

        result.Append(parameterInfo.GetUsageDetailText());
        result.Append(optionInfo.GetUsageDetailText());
      }
      finally
      {
        PropertyService.ResetCurrentCommand();
      }

      return result.ToString();
    }

    /// <inheritdoc />
    public void RegisterCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter setter, PropertyServiceCustomDataTypeValidator validator)
    {
      AddCustomDataTypeHandler(type, setter, validator);
    }

    /// <summary>
    /// Parses tokenize items.
    /// </summary>
    /// <param name="items">Tokenize items to parse.</param>
    /// <returns>Parse result.</returns>
    private ParseResult ParseItems(IList<TokenizeItem> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException(nameof(items));
      }

      var result = new ParseResult();
      object? command = null;
      object? helpOption = null;
      var commandName = string.Empty;

      try
      {
        var analysisResult = AnalyzeItems(items);

        if (analysisResult.Failure)
        {
          result.AddErrors(analysisResult.Errors);
          return result;
        }

        uint ordinalNumber = 1;

        for (var i = 0; i < items.Count; i++)
        {
          var item = items[i];
          var nextItem = (i + 1) < items.Count ? items[i + 1] : null;

          var prop = DetermineHelpOptionProperty(item);

          if (prop != null)
          {
            helpOption = PropertyService.GetGlobalPropertyValue(prop);
            continue;
          }

          // check if item refers to an option
          prop = DetermineOptionProperty(item);

          if (prop != null)
          {
            // check if item contains directly a value or not
            if (string.IsNullOrEmpty(item.Value))
            {
              // special handling of boolean options: Either no value (treat as true) or optional following value
              if (prop.PropertyType == typeof(bool))
              {
                // check if next item (if available) is a boolean value (use it) or not (ignore it)
                if (PropertyService.IsValidValue(prop, nextItem?.Name))
                {
                  PropertyService.SetPropertyValue(prop, nextItem?.Name);
                  i++;
                }
                else
                {
                  PropertyService.SetPropertyValue(prop, true.ToLowerString());
                }
              }
              else
              {
                // option refers not to a boolean - validate value (next item)
                if (PropertyService.IsValidValue(prop, nextItem?.Name))
                {
                  PropertyService.SetPropertyValue(prop, nextItem?.Name);
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
              if (PropertyService.IsValidValue(prop, item.Value))
              {
                PropertyService.SetPropertyValue(prop, item.Value);
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
            // check if item refers to a command
            prop = DetermineCommandProperty(item);

            if (prop != null)
            {
              command = PropertyService.GetPropertyValue(prop);
              commandName = item.Name;

              // indicate the current used command
              PropertyService.SetCurrentCommand(command);
              continue;
            }

            if (prop == null)
            {
              // check if item refers to a parameter
              prop = DetermineParameterProperty(ordinalNumber);

              if (prop == null)
              {
                // item neither refers to an option nor a parameter
                result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat, item.Name, item.Value, "Argument does not match any option or parameter"));
                continue;
              }
              else
              {
                if (PropertyService.IsValidValue(prop, item.Name))
                {
                  PropertyService.SetPropertyValue(prop, item.Name);
                }
                else
                {
                  result.AddError(new ParseError(ParseErrorType.InvalidParameterValue, prop.Name, item.Name));
                  continue;
                }

                ordinalNumber++;
              }
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
      catch (Exception ex)
      {
        result.AddError(new ParseError(ParseErrorType.UnknownError, Resources.NotAvailableShortName, null, ex.Message));
      }

      if (result.Success && helpOption != null)
      {
        if (command == null)
        {
          ExecuteUsage?.Invoke(new UsageEventArgs());
        }
        else
        {
          ExecuteUsage?.Invoke(new UsageEventArgs(commandName));
        }
      }
      else if (result.Success && command != null)
      {
        ExecuteCommand?.Invoke(new CommandEventArgs(commandName, command));

        if (command is ICommand cmd)
        {
          cmd.ExecuteCommand();
        }
      }

      return result;
    }

    /// <summary>
    /// Validates the tokenized items and related rules for given configuration
    /// </summary>
    /// <param name="items">Tokenite items.</param>
    /// <returns>Validation result.</returns>
    private AnalysisResult AnalyzeItems(IEnumerable<TokenizeItem> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException(nameof(items));
      }

      var result = new AnalysisResult();

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

      // check if help option is available
      // TODO: Add HelpOption attribute

      // check if command is configured and used
      result.HasCommandsDefined = PropertyService.HasCommands();

      // if comman


      //// check if parameter is defined if commands are
      //if (result.CommandsDefined && PropertyService.HasParameters())
      //{
      //  result.AddError(new ParseError(ParseErrorType.))
      //}


      return result;
    }

    /// <summary>
    /// Gets an option property for given tokenize item.
    /// </summary>
    /// <param name="item">Tokenize item.</param>
    /// <returns>Option property for given tokenize item or <see langword="null" /> if not found.</returns>
    private PropertyInfo? DetermineOptionProperty(TokenizeItem item)
    {
      // check if current token is an option
      var itemName = item.Name.TrimStart(new[] { TokenizeOptions.ArgumentOptionLongNameIndicator,
                                                 TokenizeOptions.ArgumentOptionAlternativeNameIndicator,
                                                 TokenizeOptions.ArgumentOptionDefaultNameIndicator
                                               });

      return PropertyService.GetProperties()
                            .FirstOrDefault(p => p.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() is OptionAttribute option &&
                                                 (option.Name == itemName ||
                                                  option.AlternativeName == itemName ||
                                                  option.LongName == itemName));
    }

    /// <summary>
    /// Gets a help option property for given tokenize item.
    /// </summary>
    /// <param name="item">Tokenize item.</param>
    /// <returns>Help option property for given tokenize item or <see langword="null" /> if not found.</returns>
    private PropertyInfo? DetermineHelpOptionProperty(TokenizeItem item)
    {
      // check if current token is an option
      var itemName = item.Name.TrimStart(new[] { TokenizeOptions.ArgumentOptionLongNameIndicator,
                                                 TokenizeOptions.ArgumentOptionAlternativeNameIndicator,
                                                 TokenizeOptions.ArgumentOptionDefaultNameIndicator
                                               });

      return PropertyService.GetGlobalProperties()
                            .FirstOrDefault(p => p.GetCustomAttributes(typeof(HelpOptionAttribute), true).FirstOrDefault() is HelpOptionAttribute helpOption &&
                                                 (helpOption.Name == itemName ||
                                                  helpOption.AlternativeName == itemName ||
                                                  helpOption.LongName == itemName));
    }

    /// <summary>
    /// Checks for required properties.
    /// </summary>
    /// <returns>Parse result.</returns>
    private ParseResult CheckRequiredProperties()
    {
      var result = new ParseResult();

      result.AddErrors(PropertyService
                       .GetUnassignedRequiredOptionNames()
                       .Select(name => new ParseError(ParseErrorType.RequiredOptionValue,
                                                      name,
                                                      Resources.NotApplicableValue,
                                                      string.Format(CultureInfo.InvariantCulture,
                                                                    Resources.OptionIsMissingRequiredValueFormatErrorMessage,
                                                                    name))));

      return result;
    }

    /// <summary>
    /// Gets a command decorated property for given tokenize item.
    /// </summary>
    /// <param name="item">Tokenize item.</param>
    /// <returns>Command property for given tokenize item or <see langword="null" /> if not found.</returns>
    private PropertyInfo DetermineCommandProperty(TokenizeItem item)
    {
      return PropertyService
             .GetProperties()
             .FirstOrDefault(p => p.GetCustomAttributes(typeof(CommandAttribute), true)
                                   .FirstOrDefault() is CommandAttribute command && command.Name == item.Name);
    }

    /// <summary>
    /// Gets a parameter decorated property for given ordinal number.
    /// </summary>
    /// <param name="ordinalNumber">Current ordinal number.</param>
    /// <returns>Option property for given ordinal number or <see langword="null" /> if not found.</returns>
    private PropertyInfo DetermineParameterProperty(uint ordinalNumber)
    {
      return PropertyService
             .GetProperties()
             .FirstOrDefault(p => p.GetCustomAttributes(typeof(ParameterAttribute), true)
                                   .FirstOrDefault() is ParameterAttribute parameter && parameter.OrdinalNumber == ordinalNumber);
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

    private ParameterUsageInfo GetParameterUsageInfo()
    {
      var result = new ParameterUsageInfo();

      foreach (var property in PropertyService.GetProperties())
      {
        if (property.GetCustomAttributes(typeof(ParameterAttribute), true).FirstOrDefault() is ParameterAttribute parameter)
        {
          result.AddItem(parameter);
        }
      }

      result.Items.ToList().Sort((l, r) => l.OrdinalNumber.CompareTo(r.OrdinalNumber));

      return result;
    }

    private OptionUsageInfo GetOptionUsageInfo()
    {
      var result = new OptionUsageInfo();

      foreach (var property in PropertyService.GetProperties())
      {
        if (property.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() is OptionAttribute option)
        {
          result.AddItem(option);
        }
      }
      return result;
    }

    private CommandUsageInfo GetCommandUsageInfo()
    {
      var result = new CommandUsageInfo();

      foreach (var property in PropertyService.GetProperties())
      {
        if (property.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault() is CommandAttribute command)
        {
          result.AddItem(command);
        }
      }
      return result;
    }
  }
}
