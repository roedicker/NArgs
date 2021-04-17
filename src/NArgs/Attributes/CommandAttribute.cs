using System;

namespace NArgs.Attributes
{
  /// <summary>
  /// Attribute of a command line command.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class CommandAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAttribute" /> class.
    /// </summary>
    public CommandAttribute() : base()
    {
    }
  }
}
