using System;
using System.Globalization;
using System.Linq;
using System.Text;
using NArgs.Attributes;

namespace NArgs.Models
{
  internal class OptionUsageInfo : AttributeUsageInfo<OptionAttribute>
  {
    private const string CombinedOptionNameSeparator = " | ";
    private const int NumberOfSyntaxOptionItemsPerLine = 4;

    /// <inheritdoc />
    public override void AddItem(OptionAttribute item)
    {
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      base.AddItem(item);

      var length = 0;

      if (!string.IsNullOrWhiteSpace(item.Name))
      {
        length += TokenizeOptions.ArgumentOptionDefaultNameIndicator.Length + item.Name.Length;
      }

      if (!string.IsNullOrWhiteSpace(item.AlternativeName))
      {
        if (length != 0)
        {
          length += CombinedOptionNameSeparator.Length;
        }

        length += TokenizeOptions.ArgumentOptionAlternativeNameIndicator.Length + item.AlternativeName.Length;
      }

      if (!string.IsNullOrWhiteSpace(item.LongName))
      {
        if (length != 0)
        {
          length += CombinedOptionNameSeparator.Length;
        }

        length += TokenizeOptions.ArgumentOptionLongNameIndicator.Length + item.LongName.Length;
      }

      if (length > MaxNameLength)
      {
        MaxNameLength = length;
      }
    }

    /// <inheritdoc />
    public override string GetUsageDetailText()
    {
      if (Items.Any())
      {
        var result = new StringBuilder();

        result.AppendLine($"{Environment.NewLine}  {Resources.OptionsCapitalizedName}:");

        foreach (var option in Items)
        {
          var combinedName = new StringBuilder();

          if (!string.IsNullOrWhiteSpace(option.Name))
          {
            combinedName.Append(TokenizeOptions.ArgumentOptionDefaultNameIndicator);
            combinedName.Append(option.Name);
          }

          if (!string.IsNullOrWhiteSpace(option.AlternativeName))
          {
            if (combinedName.Length != 0)
            {
              combinedName.Append(CombinedOptionNameSeparator);
            }

            combinedName.Append(TokenizeOptions.ArgumentOptionAlternativeNameIndicator);
            combinedName.Append(option.AlternativeName);
          }

          if (!string.IsNullOrWhiteSpace(option.LongName))
          {
            if (combinedName.Length != 0)
            {
              combinedName.Append(CombinedOptionNameSeparator);
            }

            combinedName.Append(TokenizeOptions.ArgumentOptionLongNameIndicator);
            combinedName.Append(option.LongName);
          }

          var name = string.Format(CultureInfo.InvariantCulture,
                                   "  {0," + Convert.ToString(-MaxNameLength) + "}",
                                   combinedName.Length == 0 ? "n/a" : combinedName.ToString());

          result.Append($"  {name}    {option.Description}{Environment.NewLine}");
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
        var counter = 0;

        foreach (var option in Items)
        {
          var currentIndention = ++counter % NumberOfSyntaxOptionItemsPerLine == 0 ? indention : 0;
          var syntaxName = new StringBuilder();

          if (!option.Required)
          {
            syntaxName.Append('[');
          }

          if (!string.IsNullOrWhiteSpace(option.Name))
          {
            syntaxName.Append(TokenizeOptions.ArgumentOptionDefaultNameIndicator);
            syntaxName.Append(option.Name);
          }

          if (!option.Required)
          {
            syntaxName.Append(']');
          }

          var indent = string.Format(CultureInfo.InvariantCulture,
                                     "{0}{1," + Convert.ToString(currentIndention, CultureInfo.InvariantCulture) + "}",
                                     Environment.NewLine,
                                     " ");
          var separator = counter % NumberOfSyntaxOptionItemsPerLine == 0 &&
                          counter < Items.Count() ?
                          indent :
                          " ";

          result.Append($"{syntaxName}{separator}");
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
