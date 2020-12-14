using System;
using System.Collections.Generic;

using NArgs.Models;

namespace NArgs
{
  /// <summary>
  /// Defines the command tokenizer
  /// </summary>
  public interface ICommandTokenizer
  {
    /// <summary>
    /// Gets the command arguments
    /// </summary>
    IEnumerable<CommandArgsItem> Items
    {
      get;
    }

    /// <summary>
    /// Gets the default command argument option indicator
    /// </summary>
    string ArgumentOptionDefaultNameIndicator
    {
      get;
    }

    /// <summary>
    /// Gets the long name command argument option indicator
    /// </summary>
    string ArgumentOptionLongNameIndicator
    {
      get;
    }

    /// <summary>
    /// Gets the parameters as command argument list
    /// </summary>
    /// <param name="args">Arguments to tokenize</param>
    /// <returns>Command argument list of parameters</returns>
    IEnumerable<CommandArgsItem> Tokenize(string[] args);
  }
}
