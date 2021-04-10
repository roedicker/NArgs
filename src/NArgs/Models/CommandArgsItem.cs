using System;

namespace NArgs.Models
{
  /// <summary>
  /// Defines a command argument item.
  /// </summary>
  public class CommandArgsItem
  {
    /// <summary>
    /// Gets or sets the command argument item type.
    /// </summary>
    public CommandArgsItemType ItemType
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the name of the command argument.
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the value of the command argument.
    /// </summary>
    public string Value
    {
      get;
      set;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgsItem" /> class.
    /// </summary>
    public CommandArgsItem()
    {
      ItemType = CommandArgsItemType.NotSet;
      Name = string.Empty;
      Value = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgsItem" /> class with given type, name and value.
    /// </summary>
    /// <param name="itemType">Item type of the command argument.</param>
    /// <param name="name">Name of the command argument.</param>
    /// <param name="value">Value of the command argument.</param>
    public CommandArgsItem(CommandArgsItemType itemType, string name, string value)
    {
      ItemType = itemType;
      Name = name;
      Value = value;
    }
  }
}
