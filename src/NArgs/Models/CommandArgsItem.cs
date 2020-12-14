using System;

namespace NArgs.Models
{
  /// <summary>
  /// Defines a command argument item
  /// </summary>
  public class CommandArgsItem
  {
    /// <summary>
    /// Gets or sets the command argument item type
    /// </summary>
    public CommandArgsItemType ItemType
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the name of the command argument
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the value of the command argument
    /// </summary>
    public string Value
    {
      get;
      set;
    }

    /// <summary>
    /// Creates a new command argument
    /// </summary>
    public CommandArgsItem()
    {
      this.ItemType = CommandArgsItemType.NotSet;
      this.Name = String.Empty;
      this.Value = String.Empty;
    }

    /// <summary>
    /// Creates a new instance of the command argument with given type, name and value
    /// </summary>
    /// <param name="itemType">Item type of the command argument</param>
    /// <param name="name">Name of the command argument</param>
    /// <param name="value">Value of the command argument</param>
    public CommandArgsItem(CommandArgsItemType itemType, string name, string value)
    {
      this.ItemType = itemType;
      this.Name = name;
      this.Value = value;
    }
  }
}
