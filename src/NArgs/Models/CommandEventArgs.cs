using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NArgs.Models
{
  /// <summary>
  /// Arguments of a command event.
  /// </summary>
  public sealed class CommandEventArgs
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public string CommandName
    {
      get;
    }

    /// <summary>
    /// Gets the command target.
    /// </summary>
    public object CommandTarget
    {
      get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandEventArgs" /> class.
    /// </summary>
    /// <param name="name">Command name.</param>
    /// <param name="target">Command target.</param>
    public CommandEventArgs(string name, object target)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(name));
      }

      CommandName = name;
      CommandTarget = target ?? throw new ArgumentNullException(nameof(target));
    }
  }
}
