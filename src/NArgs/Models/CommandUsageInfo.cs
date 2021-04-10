using System;
using System.Globalization;
using System.Linq;
using System.Text;
using NArgs.Attributes;

namespace NArgs.Models
{
  internal class CommandUsageInfo : AttributeUsageInfo<CommandAttribute>
  {
    /// <inheritdoc />
    public override void AddItem(CommandAttribute item)
    {
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      base.AddItem(item);

      if (item.Name?.Length > MaxNameLength)
      {
        MaxNameLength = item.Name.Length;
      }
    }

    /// <inheritdoc />
    public override string GetUsageDetailText()
    {
      if (Items.Any())
      {
        var result = new StringBuilder();

        result.AppendLine($"{Environment.NewLine}  {Resources.CommandsCapitalizedName}:");

        foreach (var command in Items)
        {
          result.AppendFormat(CultureInfo.InvariantCulture,
                              "    {0, " + Convert.ToString(-MaxNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}",
                              string.IsNullOrWhiteSpace(command.Name) ? "n/a" : command.Name,
                              string.IsNullOrWhiteSpace(command.Description) ? "n/a" : command.Description,
                              Environment.NewLine);
        }

        return result.ToString();
      }
      else
      {
        return string.Empty;
      }
    }

    /// <inheritdoc />
    public override string GetUsageSyntaxText(int indention)
    {
      return Items.Any() ? " <command> " : string.Empty;
    }
  }
}
