using System;
using System.Collections.Generic;
using NArgs.Models;
using NExtents;

namespace NArgs.Services;

/// <summary>
/// Defines the console argument tokenizer.
/// </summary>
internal class ConsoleArgumentTokenizer : ICommandTokenizer
{
    /// <summary>
    /// Gets the parse options.
    /// </summary>
    protected ParseOptions Options
    {
        get;
    }

    /// <inheritdoc />
    public string ArgumentOptionDefaultNameIndicator
    {
        get
        {
            return Options.ArgumentOptionDefaultNameIndicator;
        }
    }

    /// <inheritdoc />
    public string ArgumentOptionLongNameIndicator
    {
        get
        {
            return Options.ArgumentOptionLongNameIndicator;
        }
    }

    /// <inheritdoc />
    public IEnumerable<CommandArgsItem> Items
    {
        get;
        private set;
    }

    /// <summary>
    /// Initializes a new instance of the console argument tokenizer.
    /// </summary>
    public ConsoleArgumentTokenizer()
    {
        Items = new List<CommandArgsItem>();
        Options = new ParseOptions();
    }

    /// <summary>
    /// Initializes a new instance of the console argument tokenizer with initial parse options.
    /// </summary>
    /// <param name="options">Parse options.</param>
    /// <exception cref="ArgumentNullException">No parse options provided.</exception>
    public ConsoleArgumentTokenizer(ParseOptions options) : this()
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public IEnumerable<CommandArgsItem> Tokenize(string[] args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        var result = new List<CommandArgsItem>();
        var lastArgument = string.Empty;

        foreach (var arg in args)
        {
            var argument = arg.Trim();

            if (string.IsNullOrWhiteSpace(lastArgument))
            {
                lastArgument = argument;
            }
            else
            {
                switch (ProcessArguments(result, lastArgument, argument))
                {
                    case 0:
                        break;

                    case 1:
                        lastArgument = argument;
                        break;

                    case 2:
                        lastArgument = string.Empty;
                        break;

                    default:
                        break;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(lastArgument))
        {
            if (ProcessArguments(result, lastArgument, null) != 1)
            {
                throw new Exception(Resources.UnknownErrorMessage);
            }
        }

        Items = result;

        return result;
    }

    /// <summary>
    /// Processes arguments.
    /// </summary>
    /// <param name="collection">Collection of available command argument items.</param>
    /// <param name="arg1">First argument.</param>
    /// <param name="arg2">Second argument.</param>
    /// <returns>Indicator of processing the arguments.</returns>
    private int ProcessArguments(List<CommandArgsItem> collection,
        string arg1,
        string arg2)
    {
        int result;

        if (!string.IsNullOrWhiteSpace(arg1) && arg1.StartsWith(Options.GetArgumentOptionNameIndicators(), StringComparison.InvariantCultureIgnoreCase))
        {
            var optionPositionOfValueSeparator = arg1.IndexOf(Options.ArgumentOptionValueIndicator, StringComparison.OrdinalIgnoreCase);
            string optionName;
            string optionValue;

            if (optionPositionOfValueSeparator == -1)
            {
                // option name in arg1 available only - check arg2 for value
                optionName = arg1;

                if (!string.IsNullOrWhiteSpace(arg2)
                    && !arg2.StartsWith(Options.GetArgumentOptionNameIndicators(), StringComparison.InvariantCultureIgnoreCase))
                {
                    // arg2 contains a valid value
                    optionValue = arg2.Trim(Options.ArgumentQuotationCharacter);
                    result = 2;
                }
                else
                {
                    // arg2 contains either another option or has no value
                    optionValue = string.Empty;
                    result = 1;
                }
            }
            else
            {
                // option name and value in arg1 available - get value from remaining chunks
#pragma warning disable IDE0057 // Use range operator - is not defined for .NET standard
                optionName = arg1.Substring(0, optionPositionOfValueSeparator);
                optionValue = arg1.Substring(optionPositionOfValueSeparator + 1).Trim(Options.ArgumentQuotationCharacter);
#pragma warning restore IDE0057 // Use range operator
                result = 1;
            }

            collection.Add(CreateCommandArgsItem(optionName, optionValue));
        }
        else
        {
            collection.Add(CreateCommandArgsItem(arg1, string.Empty));
            result = 1;
        }

        return result;
    }

    /// <summary>
    /// Creates a command argument item.
    /// </summary>
    /// <param name="name">Name of the command argument item.</param>
    /// <param name="value">Value of the command argument item.</param>
    /// <returns>Created command argument item.</returns>
    /// <exception cref="ArgumentNullException">No command name provided.</exception>
    internal CommandArgsItem CreateCommandArgsItem(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        name = name.Trim(Options.ArgumentQuotationCharacter).Trim();

        CommandArgsItemType itemType;

        if (name.StartsWith(Options.ArgumentOptionLongNameIndicator, StringComparison.Ordinal))
        {
            itemType = CommandArgsItemType.OptionLongName;
            name = name.TrimStart(Options.ArgumentOptionLongNameIndicator);
        }
        else if (name.StartsWith(Options.GetArgumentOptionNameIndicators()))
        {
            itemType = CommandArgsItemType.Option;
            name = name.TrimStart(Options.GetArgumentOptionNameIndicators());
        }
        else
        {
            itemType = CommandArgsItemType.Parameter;
        }

        if (!string.IsNullOrWhiteSpace(value))
        {
            value = value.Trim(Options.ArgumentQuotationCharacter).Trim();
        }

        return new CommandArgsItem(itemType, name, value);
    }
}
