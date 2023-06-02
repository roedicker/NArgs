using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NArgs.Attributes;
using NArgs.Exceptions;
using NArgs.Models;
using StringCollection = NCollections.StringCollection;

namespace NArgs;

/// <summary>
/// Defines the base command line parser.
/// </summary>
public abstract class CommandLineParser
{
    /// <summary>
    /// Gets or sets the property service.
    /// </summary>
    protected IPropertyService PropertyService { get; }

    /// <summary>
    /// Gets or sets the tokenizer.
    /// </summary>
    protected ICommandTokenizer Tokenizer { get; }

    /// <summary>
    /// Gets the options of the command line parser.
    /// </summary>
    public virtual ParseOptions Options { get; }

    /// <summary>
    /// Current registered command action to call for a matching command.
    /// </summary>
    protected CommandAction CommandAction;

    /// <summary>
    /// Creates a new instance of the base command line parser.
    /// </summary>
    /// <param name="propertyService">Property service of the command line parse.</param>
    /// <param name="tokenizer">Command argument tokenizer.</param>
    protected CommandLineParser(IPropertyService propertyService, ICommandTokenizer tokenizer)
    {
        PropertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
        Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
    }

    /// <summary>
    /// Parses the command line arguments.
    /// </summary>
    /// <returns>Parse result.</returns>
    internal ParseResult ParseCommandLine()
    {
        var result = new ParseResult();
        var hasCommands = PropertyService.HasCommands();
        var parameterNumber = 1;

        try
        {
            // check plausibility: Options must follow parameters 
            var optionsAvailable = false;

            foreach (var item in Tokenizer.Items)
            {
                if (item.ItemType == CommandArgsItemType.Parameter)
                {
                    if (optionsAvailable)
                    {
                        result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat,
                                                       item.Name,
                                                       item.Value,
                                                       Resources.ParametersMustPrecedeAnyOptionsErrorMessage));
                    }
                }
                else
                {
                    optionsAvailable = true;
                }
            }

            // process parameters
            string commandName = null;
            object commandObject = null;

            foreach (var item in Tokenizer.Items)
            {
                try
                {
                    if (commandName is null)
                    {
                        foreach (var info in PropertyService.GetProperties())
                        {
                            // check if parameter refers to a command
                            if (info.GetCustomAttributes(typeof(Command), true).FirstOrDefault() is Command command
                                && (string.Compare(command.Name, item.Name, StringComparison.OrdinalIgnoreCase) == 0
                                || string.Compare(command.LongName, item.Name, StringComparison.OrdinalIgnoreCase) == 0))
                            {
                                if (commandName is null)
                                {
                                    commandName = command.Name;
                                    commandObject = PropertyService.GetCommandByName(info.Name);
                                }
                                else
                                {
                                    result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat,
                                                                   item.Name,
                                                                   item.Value,
                                                                   Resources.MultipleCommandsUsedErrorMessage));
                                }

                                break;
                            }

                            // check for parameter
                            if (info.GetCustomAttributes(typeof(Parameter), true).FirstOrDefault() is Parameter parameter
                                && parameter.OrdinalNumber == parameterNumber)
                            {
                                if (item.ItemType == CommandArgsItemType.Parameter)
                                {
                                    if (commandName is null)
                                    {
                                        if (hasCommands)
                                        {
                                            result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat,
                                                                           item.Name,
                                                                           item.Value,
                                                                           Resources.ParametersNotAllowedBesideCommandsErrorMessage));
                                        }
                                        else
                                        {
                                            PropertyService.SetPropertyValue(info, item.Name);
                                        }
                                    }
                                    else
                                    {
                                        PropertyService.SetCommandPropertyValue(info, commandName, item.Name);
                                    }

                                    parameterNumber++;
                                    break;
                                }
                            }
                        }

                        if (commandName is null
                            && hasCommands
                            && item == Tokenizer.Items.First())
                        {
                            result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat,
                               item.Name,
                               item.Value,
                               Resources.CommandDoesNotExistErrorMessage));
                        }
                    }
                    else
                    {
                        foreach (var info in PropertyService.GetCommandProperties(commandName))
                        {
                            // check for parameter
                            if (info.GetCustomAttributes(typeof(Parameter), true).FirstOrDefault() is Parameter parameter
                                && parameter.OrdinalNumber == parameterNumber)
                            {
                                if (item.ItemType == CommandArgsItemType.Parameter)
                                {
                                    if (commandName is null)
                                    {
                                        PropertyService.SetPropertyValue(info, item.Name);
                                    }
                                    else
                                    {
                                        PropertyService.SetCommandPropertyValue(info, commandName, item.Name);
                                    }

                                    parameterNumber++;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(new ParseError(ParseErrorType.UnknownError,
                                                   item.Name,
                                                   item.Value,
                                                   ex.Message));
                }
            }

            // process options
            foreach (var info in PropertyService.GetProperties())
            {
                try
                {
                    var optionProcessed = false;

                    if (info.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
                    {
                        foreach (var item in Tokenizer.Items)
                        {
                            switch (item.ItemType)
                            {
                                case CommandArgsItemType.Option:
                                    if (option.Name == item.Name
                                        || option.AlternativeName == item.Name)
                                    {
                                        if (PropertyService.IsValidOptionValue(item.Name, item.Value))
                                        {
                                            PropertyService.SetPropertyValue(info, item.Value);
                                            optionProcessed = true;
                                        }
                                        else
                                        {
                                            result.AddError(new ParseError(ParseErrorType.InvalidOptionValue,
                                                                           item.Name,
                                                                           item.Value,
                                                                           string.Format(CultureInfo.InvariantCulture,
                                                                                         Resources.ValueIsInvalidForOptionFormatErrorMessage,
                                                                                         item.Value,
                                                                                         item.Name)));
                                        }
                                    }

                                    break;

                                case CommandArgsItemType.OptionLongName:
                                    if (option.LongName == item.Name)
                                    {
                                        if (PropertyService.IsValidOptionValue(item.Name, item.Value))
                                        {
                                            PropertyService.SetPropertyValue(info, item.Value);
                                            optionProcessed = true;
                                        }
                                        else
                                        {
                                            result.AddError(new ParseError(ParseErrorType.InvalidOptionValue,
                                                                           item.Name,
                                                                           item.Value,
                                                                           string.Format(CultureInfo.InvariantCulture,
                                                                                         Resources.ValueIsInvalidForOptionFormatErrorMessage,
                                                                                         item.Value,
                                                                                         item.Name)));
                                        }
                                    }

                                    break;
                            }

                            if (optionProcessed)
                            {
                                break;
                            }
                        }
                    }

                    if (!optionProcessed
                        && PropertyService.IsRequired(info))
                    {
                        var optionName = PropertyService.GetOptionName(info);

                        result.AddError(new ParseError(ParseErrorType.RequiredOptionValue,
                                                       optionName,
                                                       null,
                                                       string.Format(CultureInfo.InvariantCulture,
                                                                     Resources.OptionIsMissingRequiredValueFormatErrorMessage,
                                                                     optionName)));
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(new ParseError(ParseErrorType.UnknownError,
                                                   Resources.NotAvailableShortName,
                                                   null,
                                                   ex.Message));
                }
            }

            // trigger command action if command has been found and delegate has been set
            if (commandName is not null)
            {
                CommandAction?.Invoke(commandName, commandObject);
            }
        }
        catch (InvalidCommandArgsFormatException ex)
        {
            result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat,
                                           ex.ItemName,
                                           null,
                                           ex.Message));
        }
        catch (Exception ex)
        {
            result.AddError(new ParseError(ParseErrorType.UnknownError,
                                           Resources.NotAvailableShortName,
                                           null,
                                           ex.Message));
        }

        return result;
    }

    /// <summary>
    /// Gets the usage for the current configuration.
    /// </summary>
    /// <param name="executable">Current executable name.</param>
    /// <param name="commandName">Optional. Command name to show usage for.</param>
    /// <returns>Usage output for the current configuration.</returns>
    internal string GetUsage(string executable, string commandName = null)
    {
        var result = new StringBuilder();
        var commands = new List<KeyValuePair<string, string>>();
        var parameters = new List<Parameter>();
        var options = new List<KeyValuePair<string, string>>();
        var optionsSyntaxUsage = new StringCollection();
        var maxCommandCompleteNameLength = 0;
        var maxParameterNameLength = 0;
        var maxOptionCompleteNameLength = 0;
        var properties = commandName is null
                         ? PropertyService.GetProperties()
                         : PropertyService.GetCommandProperties(commandName);
        var indentions = new StringBuilder();

        for (var i = 0; i < executable.Length + 3; i++)
        {
            indentions.Append(' ');
        }

        foreach (var info in properties)
        {
            if (info.GetCustomAttributes(typeof(Command), true).FirstOrDefault() is Command command)
            {
                var commandNames = new StringCollection();

                if (!string.IsNullOrWhiteSpace(command.Name))
                {
                    commandNames.AddDistinct(command.Name);
                }

                if (!string.IsNullOrWhiteSpace(command.LongName))
                {
                    commandNames.AddDistinct(command.LongName);
                }

                var commandCompleteName = commandNames.ToString(" | ");

                commands.Add(new KeyValuePair<string, string>(commandCompleteName, string.IsNullOrWhiteSpace(command.Description) ? Resources.NotApplicableValue : command.Description));

                if (command.Name?.Length > maxCommandCompleteNameLength)
                {
                    maxCommandCompleteNameLength = command.Name.Length;
                }
            }
            else if (info.GetCustomAttributes(typeof(Parameter), true).FirstOrDefault() is Parameter parameter)
            {
                parameters.Add(parameter);

                if (parameter.Name?.Length > maxParameterNameLength)
                {
                    maxParameterNameLength = parameter.Name.Length;
                }
            }
            else
            {
                if (info.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
                {
                    var optionNames = new StringCollection();

                    if (!string.IsNullOrWhiteSpace(option.Name))
                    {
                        optionNames.AddDistinct($"{Tokenizer.ArgumentOptionDefaultNameIndicator}{option.Name}");
                    }

                    if (!string.IsNullOrWhiteSpace(option.AlternativeName))
                    {
                        optionNames.AddDistinct($"{Tokenizer.ArgumentOptionDefaultNameIndicator}{option.AlternativeName}");
                    }

                    if (!string.IsNullOrWhiteSpace(option.LongName))
                    {
                        optionNames.AddDistinct($"{Tokenizer.ArgumentOptionLongNameIndicator}{option.LongName}");
                    }

                    var optionCompleteName = optionNames.ToString(" | ");

                    if (option.Required)
                    {
                        if (info.PropertyType.FullName == PropertyTypeFullName.Boolean)
                        {
                            optionsSyntaxUsage.Add($"{optionCompleteName}");
                        }
                        else
                        {
                            optionsSyntaxUsage.Add($"{optionCompleteName} <{option.UsageTypeDisplayName}>");
                        }
                    }
                    else
                    {
                        if (info.PropertyType.FullName == PropertyTypeFullName.Boolean)
                        {
                            optionsSyntaxUsage.Add($"[{optionCompleteName}]");
                        }
                        else
                        {
                            optionsSyntaxUsage.Add($"[{optionCompleteName} <{option.UsageTypeDisplayName}>]");
                        }
                    }

                    options.Add(new KeyValuePair<string, string>(optionCompleteName, string.IsNullOrWhiteSpace(option.Description) ? Resources.NotApplicableValue : option.Description));

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

        // generate syntax for command name
        if (commandName is not null)
        {
            result.Append($"{commandName} ");
        }

        // generate syntax for parameters
        if (parameters.Any())
        {
            foreach (var parameter in parameters)
            {
                result.AppendFormat(CultureInfo.InvariantCulture,
                                    "<{0}> ",
                                    string.IsNullOrWhiteSpace(parameter.Name) ?
                                    Resources.NotApplicableValue :
                                    parameter.Name);
            }
        }

        // generate syntax for options
        if (options.Any())
        {
            result.Append($"{optionsSyntaxUsage.ToString($"{Environment.NewLine}{indentions}")}");
        }

        if (options.Any()
           || parameters.Any())
        {
            result.Append($"{Environment.NewLine}");
        }

        if (commands.Any())
        {
            if (parameters.Any()
                || options.Any())
            {
                result.AppendLine($"{indentions}<command> [<args>]");

            }
            else
            {
                result.AppendLine($"<command> [<args>]");
            }
        }

        // generate usage for parameters
        if (parameters.Any())
        {
            result.AppendLine($"{Environment.NewLine}{Resources.ParametersCapitalizedName}:");

            foreach (var parameter in parameters)
            {
                result.AppendFormat(CultureInfo.InvariantCulture,
                                    "  {0, " + Convert.ToString(-maxParameterNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}",
                                    string.IsNullOrWhiteSpace(parameter.Name) ?
                                    Resources.NotApplicableValue :
                                    parameter.Name,
                                    string.IsNullOrWhiteSpace(parameter.Description) ?
                                    Resources.NotApplicableValue :
                                    parameter.Description,
                                    Environment.NewLine);
            }
        }


        // generate usage for options
        if (options.Any())
        {
            result.AppendLine($"{Environment.NewLine}{Resources.OptionsCapitalizedName}:");

            foreach (var option in options)
            {
                result.AppendFormat(CultureInfo.InvariantCulture,
                                    "  {0, " + Convert.ToString(-maxOptionCompleteNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}",
                                    option.Key,
                                    option.Value,
                                    Environment.NewLine);
            }
        }

        // generate usage for commands
        if (commands.Any())
        {
            result.AppendLine($"{Environment.NewLine}{Resources.CommandsCapitalizedName}:");

            foreach (var command in commands)
            {
                result.AppendFormat(CultureInfo.InvariantCulture,
                                     "  {0, " + Convert.ToString(-maxCommandCompleteNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}",
                                     command.Key,
                                     command.Value,
                                     Environment.NewLine);
            }
        }


        return result.ToString();
    }
}
