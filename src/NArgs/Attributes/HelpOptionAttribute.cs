using System;

namespace NArgs.Attributes
{
  /// <summary>
  /// Defines attribute for a command line help option.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class HelpOptionAttribute : OptionAttribute
  {
  }
}
