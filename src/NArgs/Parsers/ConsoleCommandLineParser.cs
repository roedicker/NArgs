using System;
using System.Collections.Generic;
using NArgs.Models;
using NArgs.Services;

namespace NArgs;

/// <summary>
/// Defines the console command line argument parser.
/// </summary>
public class ConsoleCommandLineParser : CommandLineParser, IArgumentParser
{
    /// <inheritdoc />
    public override ParseOptions Options { get; }

    /// <summary>
    /// Creates a new instance of the console command line argument parser.
    /// </summary>
    public ConsoleCommandLineParser() : base(new PropertyService(), new ConsoleArgumentTokenizer())
    {
        Options = new ParseOptions();
    }

    /// <summary>
    /// Creates a new instance of the console command line argument parser with custom parse options.
    /// </summary>
    public ConsoleCommandLineParser(ParseOptions options) : base(new PropertyService(options), new ConsoleArgumentTokenizer(options))
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public ParseResult ParseArguments(object config, string[] args)
    {
        Tokenizer.Tokenize(args ?? throw new ArgumentNullException(nameof(args)));
        PropertyService.Init(config ?? throw new ArgumentNullException(nameof(config)));

        return ParseCommandLine();
    }

    /// <inheritdoc />
    public ParseResult ParseArguments(object config, string args)
    {
        Tokenizer.Tokenize(Tokenize(args ?? throw new ArgumentNullException(nameof(args)), Options.ArgumentQuotationCharacter, Options.ArgumentDefaultSeparatorCharacter));
        PropertyService.Init(config ?? throw new ArgumentNullException(nameof(config)));

        return ParseCommandLine();
    }

    /// <inheritdoc />
    public ParseResult ParseArguments(object config,
        string args,
        CommandAction commandAction)
    {
        CommandAction = commandAction;

        return ParseArguments(config, args);
    }

    /// <inheritdoc />
    public ParseResult ParseArguments(object config,
        string[] args,
        CommandAction commandAction)
    {
        CommandAction = commandAction;

        return ParseArguments(config, args);
    }

    /// <inheritdoc />
    public void RegisterCustomDataTypeHandler(Type type,
        PropertyServiceCustomDataTypeGetter setter,
        PropertyServiceCustomDataTypeValidator validator)
    {
        AddCustomDataTypeHandler(type, setter, validator);
    }

    /// <summary>
    /// Adds a custom data-type handler
    /// </summary>
    /// <param name="type">Data-type to be handled</param>
    /// <param name="setter">Setter of the custom data-type</param>
    /// <param name="validator">Validator of the custom data-type</param>
    internal void AddCustomDataTypeHandler(Type type,
        PropertyServiceCustomDataTypeGetter setter,
        PropertyServiceCustomDataTypeValidator validator)
    {
        PropertyService.AddCustomDataTypeHandler(type, setter, validator);
    }

    /// <inheritdoc />
    public string GetUsage(object config,
        string executable,
        string commandName = null)
    {
        Tokenizer.Tokenize(Tokenize(string.Empty));
        PropertyService.Init(config);

        return base.GetUsage(executable, commandName);
    }

    /// <summary>
    /// Tokenizes a string regarding quoted parts.
    /// </summary>
    /// <param name="s">String to be tokenized.</param>
    /// <param name="quoteChar">Quote control character.</param>
    /// <param name="separator">Additional separator (beside space and tabulator).</param>
    /// <returns>string enumeration with all found string tokens.</returns>
    private string[] Tokenize(string s, char quoteChar = '"', char separator = ' ')
    {
        var result = new List<string>();
        var tokenBuilder = new System.Text.StringBuilder();
        var isQuote = false;

        foreach (var c in s.ToCharArray())
        {
            if (c == ' ' ||
                c == (char)9 ||
                c == separator)
            {
                // spaces, tabs and (optional) an additional delimiter
                // separates the tokens: only if not quoted
                if (!isQuote)
                {
                    // only create a new token if current entry is not empty. This
                    // can occur if several separators appear in a sequence
                    if (tokenBuilder.Length > 0)
                    {
                        result.Add(tokenBuilder.ToString());
                        tokenBuilder = new System.Text.StringBuilder();
                    }
                }
                else
                {
                    // if quoted, it is part of the token, so append it
                    tokenBuilder.Append(c);
                }
            }
            else if (c == quoteChar)
            {
                // always add quote character to current token
                tokenBuilder.Append(c);
                isQuote = !isQuote;
            }
            else
            {
                tokenBuilder.Append(c);
            }
        }

        // add the last token to the collection - only if not empty
        if (tokenBuilder.Length > 0)
        {
            result.Add(tokenBuilder.ToString());
        }

        return result.ToArray();
    }
}
