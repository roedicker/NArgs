using System;
using System.Globalization;
using System.Linq;
using System.Text;

using NArgs.Attributes;

namespace NArgs.Models
{
  internal class ParameterUsageInfo : AttributeUsageInfo<ParameterAttribute>
  {
    /// <inheritdoc />
    public override void AddItem(ParameterAttribute item)
    {
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

        result.AppendLine($"{Environment.NewLine}  {Resources.ParametersCapitalizedName}:");

        foreach (var parameter in Items)
        {
          result.AppendFormat(CultureInfo.InvariantCulture,
                              "    {0, " + Convert.ToString(-MaxNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}",
                              string.IsNullOrWhiteSpace(parameter.Name) ? "n/a" : parameter.Name,
                              string.IsNullOrWhiteSpace(parameter.Description) ? "n/a" : parameter.Description,
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
      if (Items.Any())
      {
        var result = new StringBuilder();

        foreach (var parameter in Items)
        {
          result.AppendFormat(CultureInfo.InvariantCulture, "<{0}> ", string.IsNullOrWhiteSpace(parameter.Name) ? Resources.NotApplicableValue : parameter.Name);
        }

        return result.ToString();
      }
      else
      {
        return string.Empty;
      }
    }
  }
}
