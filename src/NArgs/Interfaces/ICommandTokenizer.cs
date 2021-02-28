using System;
using System.Collections.Generic;

using NArgs.Models;

namespace NArgs
{
  /// <summary>
  /// Defines the command tokenizer.
  /// </summary>
  public interface ICommandTokenizer
  {

    /// <summary>
    /// Gets the command tokenizer options.
    /// </summary>
    TokenizeOptions Options
    {
      get;
    }

    /// <summary>
    /// Gets the tokenized items based on a command argument list.
    /// </summary>
    /// <param name="args">Arguments to tokenize</param>
    /// <returns>Tokenize items</returns>
    IList<TokenizeItem> Tokenize(IEnumerable<string> args);

    /// <summary>
    /// Gets the tokenized items based on command argument list.
    /// </summary>
    /// <param name="args">Arguments to tokenize</param>
    /// <returns>Tokenize items</returns>
    IList<TokenizeItem> Tokenize(string args);
  }
}
