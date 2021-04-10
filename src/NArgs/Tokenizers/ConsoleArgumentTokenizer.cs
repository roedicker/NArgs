using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using NArgs.Enumerations;
using NArgs.Models;

using NExtents;

[assembly: InternalsVisibleToAttribute("NArgsTest")]
namespace NArgs.Services
{
  /// <summary>
  /// Console argument tokenizer.
  /// </summary>
  internal sealed class ConsoleArgumentTokenizer : ICommandTokenizer
  {
    /// <inheritdoc />
    public TokenizeOptions Options
    {
      get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleArgumentTokenizer" /> class.
    /// </summary>
    public ConsoleArgumentTokenizer()
    {
      Options = new TokenizeOptions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleArgumentTokenizer"/> class with options.
    /// </summary>
    /// <param name="options">Custom options to use for this console argument tokenizer.</param>
    public ConsoleArgumentTokenizer(TokenizeOptions options) : this()
    {
      Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public IList<TokenizeItem> Tokenize(IEnumerable<string> args)
    {
      if (args == null)
      {
        throw new ArgumentNullException(nameof(args));
      }

      var result = new List<TokenizeItem>();
      var state = TokenizeState.ScanName;

      foreach (var arg in args)
      {
        // remove leading or trailing whitespaces
        var argument = arg.Trim();

        // check for option
        var isOption = argument.StartsWith(Options.ArgumentOptionNameIndicators);

        var tokenBuilder = new StringBuilder();

        try
        {
          foreach (var character in argument)
          {
            if (state == TokenizeState.ScanName)
            {
              // check if current argument is an option
              if (isOption)
              {
                if (Options.ArgumentOptionValueSeparators.Contains(character))
                {
                  AddNameItem(result, tokenBuilder);
                  state = TokenizeState.ScanValue;
                  continue;
                }
                else
                {
                  tokenBuilder.Append(character);
                }
              }
              else if (character == Options.QuotationCharacter)
              {
                state = TokenizeState.ScanQuotedName;
                continue;
              }
              else
              {
                tokenBuilder.Append(character);
              }
            }

            if (state == TokenizeState.ScanBeginValue)
            {
              if (Options.Seperators.Contains(character))
              {
                continue;
              }
              else if (character == Options.QuotationCharacter)
              {
                state = TokenizeState.ScanQuotedName;
                continue;
              }
              else if (Options.ArgumentOptionValueSeparators.Contains(character))
              {
                state = TokenizeState.ScanValue;
                continue;
              }
              else
              {
                tokenBuilder.Append(character);
                state = TokenizeState.ScanName;
                continue;
              }
            }

            if (state == TokenizeState.ScanValue)
            {
              if (character == Options.QuotationCharacter)
              {
                if (tokenBuilder.Length == 0)
                {
                  state = TokenizeState.ScanQuotedValue;
                  continue;
                }
                else
                {
                  AddFailedItem(result, argument, TokenizeErrorType.InvalidCharacterInName, "Unexpected quotation in value detected");
                  state = TokenizeState.ScanName;
                  break;
                }
              }
              else
              {
                tokenBuilder.Append(character);
              }
            }

            if (state == TokenizeState.ScanQuotedName)
            {
              if (character == Options.QuotationCharacter)
              {
                AddNameItem(result, tokenBuilder, false);
                state = TokenizeState.ScanName;
                continue;
              }
              else
              {
                tokenBuilder.Append(character);
              }
            }

            if (state == TokenizeState.ScanQuotedValue)
            {
              if (character == Options.QuotationCharacter)
              {
                AddValueItem(result, tokenBuilder);
                state = TokenizeState.ScanName;
                continue;
              }
              else
              {
                tokenBuilder.Append(character);
              }
            }
          }

          if (tokenBuilder.Length > 0)
          {
            switch (state)
            {
              case TokenizeState.ScanName:
                AddNameItem(result, tokenBuilder);

                if (isOption)
                {
                  state = TokenizeState.ScanBeginValue;
                }

                break;

              case TokenizeState.ScanBeginValue:
                AddNameItem(result, tokenBuilder);
                state = TokenizeState.ScanName;
                break;

              case TokenizeState.ScanValue:
                AddValueItem(result, tokenBuilder);
                state = TokenizeState.ScanName;
                break;

              case TokenizeState.ScanQuotedName:
              case TokenizeState.ScanQuotedValue:
                AddFailedItem(result, argument, TokenizeErrorType.IncompleteQuotation, "Quoted item is incomplete");
                state = TokenizeState.ScanName;
                break;

              default:
                throw new NotSupportedException($@"State ""{state}"" is not supported");
            }
          }
        }
        catch (Exception ex)
        {
          switch (state)
          {
            case TokenizeState.ScanName:
            case TokenizeState.ScanBeginValue:
            case TokenizeState.ScanQuotedName:
              AddFailedItem(result, tokenBuilder.ToString(), TokenizeErrorType.Unknown, ex.Message);
              break;

            case TokenizeState.ScanValue:
            case TokenizeState.ScanQuotedValue:
              AddFailedItem(result, result.Last().Name, TokenizeErrorType.Unknown, ex.Message);
              break;

            default:
              AddFailedItem(result, tokenBuilder.ToString(), TokenizeErrorType.Unknown, ex.Message);
              break;
          }
        }
      }

      return result;
    }

    /// <inheritdoc />
    public IList<TokenizeItem> Tokenize(string args)
    {
      if (string.IsNullOrEmpty(args))
      {
        return new List<TokenizeItem>();
      }

      return Tokenize(args.Tokenize(Options.QuotationCharacter));
    }

    /// <summary>
    /// Adds a new successful tokenize item to a list.
    /// </summary>
    /// <param name="list">Target list.</param>
    /// <param name="builder">Token builder containing the token.</param>
    /// <param name="trimValue">Optional. Indicator whether the value in the builder has to be trimmed or not. Default vaue is <see langword="true" />.</param>
    /// <returns>New and empty <see cref="StringBuilder" /> instance.</returns>
    private void AddNameItem(IList<TokenizeItem> list, StringBuilder builder, bool trimValue = true)
    {
      var name = trimValue ? builder.ToString().Trim(Options.Seperators) : builder.ToString();

      if (!string.IsNullOrWhiteSpace(name))
      {
        list.Add(new TokenizeItem(name, null));
      }

      builder.Clear();
    }

    /// <summary>
    /// Adds a new successful tokenize value to a the last list item.
    /// </summary>
    /// <param name="list">Target list.</param>
    /// <param name="builder">Token builder containing the token.</param>
    private void AddValueItem(IList<TokenizeItem> list, StringBuilder builder)
    {
      var value = builder.ToString();

      list.Last().Value = string.IsNullOrEmpty(value) ? null : value.Trim(Options.Seperators);

      builder.Clear();
    }

    /// <summary>
    /// Adds a new failed tokenize item to a list.
    /// </summary>
    /// <param name="list">Target list.</param>
    /// <param name="name">Name of the kokenize item to add.</param>
    /// <param name="errorType">Error typpe of the failed item.</param>
    /// <param name="errorMessage">Error message of the failed item.</param>
    /// <returns>New and empty <see cref="StringBuilder" /> instance.</returns>
    private static void AddFailedItem(IList<TokenizeItem> list, string name, TokenizeErrorType errorType, string? errorMessage = null)
    {
      list.Add(new TokenizeItem(name, errorType, errorMessage));
    }
  }
}
