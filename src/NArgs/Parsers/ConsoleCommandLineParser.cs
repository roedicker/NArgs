using System;
using System.Collections.Generic;
using NArgs.Models;
using NArgs.Services;

namespace NArgs
{
  /// <summary>
  /// Defines the console command line argument parser
  /// </summary>
  public class ConsoleCommandLineParser : CommandLineParser, IArgumentParser
  {
    /// <summary>
    /// Gets the argument parse options
    /// </summary>
    protected ParseOptions Options
    {
      get;
    }

    /// <summary>
    /// Creates a new instance of the console command line argument parser
    /// </summary>
    public ConsoleCommandLineParser() : base(new DefaultPropertyService(), new ConsoleArgumentTokenizer())
    {
      this.Options = new ParseOptions();
    }

    /// <summary>
    /// Creates a new instance of the console command line argument parser with custom parse options
    /// </summary>
    public ConsoleCommandLineParser(ParseOptions options) : base(new DefaultPropertyService(options), new ConsoleArgumentTokenizer(options))
    {
      this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Parses arguments based on configuration and an argument array
    /// </summary>
    /// <param name="config">Argument configuration</param>
    /// <param name="args">Arguments</param>
    /// <returns>Parse result for given arguments</returns>
    public ParseResult ParseArguments(object config, string[] args)
    {
      this.Tokenizer.Tokenize(args ?? throw new ArgumentNullException(nameof(args)));
      this.PropertyService.Init(config ?? throw new ArgumentNullException(nameof(config)));

      return this.ParseCommandLine();
    }

    /// <summary>
    /// Parses arguments based on configuration and arguments command line
    /// </summary>
    /// <param name="config">Argument configuration</param>
    /// <param name="args">Arguments</param>
    /// <returns></returns>
    public ParseResult ParseArguments(object config, string args)
    {
      this.Tokenizer.Tokenize(Tokenize(args ?? throw new ArgumentNullException(nameof(args)), this.Options.ArgumentQuotationCharacter, this.Options.ArgumentDefaultSeparatorCharacter));
      this.PropertyService.Init(config ?? throw new ArgumentNullException(nameof(config)));

      return this.ParseCommandLine();
    }

    /// <summary>
    /// Registers a handler for a custom data-type property
    /// </summary>
    /// <param name="type">Data-type to be handled</param>
    /// <param name="setter">Setter of the custom data-type</param>
    /// <param name="validator">Validator of the custom data-type</param>
    public void RegisterCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter setter, PropertyServiceCustomDataTypeValidator validator)
    {
      this.AddCustomDataTypeHandler(type, setter, validator);
    }

    /// <summary>
    /// Adds a custom data-type handler
    /// </summary>
    /// <param name="type">Data-type to be handled</param>
    /// <param name="setter">Setter of the custom data-type</param>
    /// <param name="validator">Validator of the custom data-type</param>
    internal void AddCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter setter, PropertyServiceCustomDataTypeValidator validator)
    {
      this.PropertyService.AddCustomDataTypeHandler(type, setter, validator);
    }

    /// <summary>
    /// Generates the output for the usage based on configuration
    /// </summary>
    /// <param name="config">Argument configuration</param>
    /// <param name="executable">Name of the executable</param>
    /// <returns></returns>
    public string GetUsage(object config, string executable)
    {
      this.Tokenizer.Tokenize(Tokenize(String.Empty));
      this.PropertyService.Init(config);

      return base.GetUsage(executable);
    }

    /// <summary>
    /// Tokenizes a string reagarding quoted parts.
    /// </summary>
    /// <param name="s">String to be tokenized.</param>
    /// <param name="quoteChar">Quote control character.</param>
    /// <param name="separator">Additional seperator (beside space and tabulator).</param>
    /// <returns>string enumeration with all found string tokens.</returns>
    private string[] Tokenize(string s, char quoteChar = '"', char separator = ' ')
    {
      List<string> Result = new List<string>();
      System.Text.StringBuilder sbToken = new System.Text.StringBuilder();

      char[] aChars = s.ToCharArray();
      bool bInQuote = false;

      foreach (char cChar in aChars)
      {
        if (cChar == ' ' || cChar == (char)9 || cChar == separator)
        {
          // spaces, tabs and (optional) an additional delimeter
          // separates the tokens: only if not quoted
          if (!bInQuote)
          {
            // only create a new token if current entry is not empty. This
            // can occur if several seperators appear in a sequence
            if (sbToken.Length > 0)
            {
              Result.Add(sbToken.ToString());
              sbToken = new System.Text.StringBuilder();
            }
          }
          else
          {
            // if quoted, it is part of the token, so append it
            sbToken.Append(cChar);
          }
        }
        else if (cChar == quoteChar)
        {
          // always add quote character to current token
          sbToken.Append(cChar);
          bInQuote = !bInQuote;
        }
        else
        {
          sbToken.Append(cChar);
        }
      }

      // add the last token to the collection - only if not empty
      if (sbToken.Length > 0)
      {
        Result.Add(sbToken.ToString());
      }

      return Result.ToArray();
    }
  }
}
