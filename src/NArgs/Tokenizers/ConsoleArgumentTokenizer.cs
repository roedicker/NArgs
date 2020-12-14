using System;
using System.Collections.Generic;

using NArgs.Models;

using NExtents;

namespace NArgs.Services
{
  internal class ConsoleArgumentTokenizer : ICommandTokenizer
  {
    protected ParseOptions Options
    {
      get;
    }

    /// <summary>
    /// Gets the default command argument option indicator
    /// </summary>
    public string ArgumentOptionDefaultNameIndicator
    {
      get
      {
        return this.Options.ArgumentOptionDefaultNameIndicator;
      }
    }

    /// <summary>
    /// Gets the long name command argument option indicator
    /// </summary>
    public string ArgumentOptionLongNameIndicator
    {
      get
      {
        return this.Options.ArgumentOptionLongNameIndicator;
      }
    }

    public IEnumerable<CommandArgsItem> Items
    {
      get;
      private set;
    }

    public ConsoleArgumentTokenizer()
    {
      this.Items = new List<CommandArgsItem>();
      this.Options = new ParseOptions();
    }

    public ConsoleArgumentTokenizer(ParseOptions options) : this()
    {
      this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public IEnumerable<CommandArgsItem> Tokenize(string[] args)
    {
      if (args == null)
      {
        throw new ArgumentNullException(nameof(args));
      }

      List<CommandArgsItem> Result = new List<CommandArgsItem>();
      string sLastArgument = String.Empty;

      foreach (string sCmdArg in args)
      {
        string sArgument = sCmdArg.Trim();

        if (String.IsNullOrWhiteSpace(sLastArgument))
        {
          sLastArgument = sArgument;
        }
        else
        {
          switch (ProcessArguments(Result, sLastArgument, sArgument))
          {
            case 0:
              break;

            case 1:
              sLastArgument = sArgument;
              break;

            case 2:
              sLastArgument = String.Empty;
              break;

            default:
              break;
          }
        }
      }

      if (!String.IsNullOrWhiteSpace(sLastArgument))
      {
        if (ProcessArguments(Result, sLastArgument, null) != 1)
        {
          throw new Exception("Unknown error");
        }
      }

      this.Items = Result;

      return Result;
    }

    private int ProcessArguments(List<CommandArgsItem> collection, string arg1, string? arg2)
    {
      int Result;

      if (!String.IsNullOrWhiteSpace(arg1) && arg1.StartsWith(this.Options.GetArgumentOptionNameIndicators(), StringComparison.InvariantCultureIgnoreCase))
      {
        int iPosOptionValueSeparator = arg1.IndexOf(this.Options.ArgumentOptionValueIndicator, StringComparison.OrdinalIgnoreCase);
        string sOptionName;
        string sOptionValue;

        if (iPosOptionValueSeparator == -1)
        {
          // option name in arg1 available only - check arg2 for value
          sOptionName = arg1;

#pragma warning disable CS8604 // Possible null reference argument.
          if (!String.IsNullOrWhiteSpace(arg2) && !arg2.StartsWith(this.Options.GetArgumentOptionNameIndicators(), StringComparison.InvariantCultureIgnoreCase))
#pragma warning restore CS8604 // Possible null reference argument.
          {
            // arg2 contains a valid value
            sOptionValue = arg2.Trim(this.Options.ArgumentQuotationCharacter);
            Result = 2;
          }
          else
          {
            // arg2 contains either another option or has no value
            sOptionValue = String.Empty;
            Result = 1;
          }
        }
        else
        {
          // option name and value in arg1 available - get value from remaining chunks
          sOptionName = arg1.Substring(0, iPosOptionValueSeparator);
          sOptionValue = arg1.Substring(iPosOptionValueSeparator + 1).Trim(this.Options.ArgumentQuotationCharacter);
          Result = 1;
        }

        collection.Add(CreateCommandArgsItem(sOptionName, sOptionValue));
      }
      else
      {
        collection.Add(CreateCommandArgsItem(arg1, String.Empty));
        Result = 1;
      }

      return Result;
    }

    internal CommandArgsItem CreateCommandArgsItem(string name, string value)
    {
      if (String.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      name = name.Trim(this.Options.ArgumentQuotationCharacter).Trim();

      CommandArgsItemType oItemType;

      if (name.StartsWith(this.Options.ArgumentOptionLongNameIndicator, StringComparison.Ordinal))
      {
        oItemType = CommandArgsItemType.OptionLongName;
        name = name.TrimStart(this.Options.ArgumentOptionLongNameIndicator);
      }
      else if (name.StartsWith(this.Options.GetArgumentOptionNameIndicators()))
      {
        oItemType = CommandArgsItemType.Option;
        name = name.TrimStart(this.Options.GetArgumentOptionNameIndicators());
      }
      else
      {
        oItemType = CommandArgsItemType.Parameter;
      }

      if (!String.IsNullOrWhiteSpace(value))
      {
        value = value.Trim(this.Options.ArgumentQuotationCharacter).Trim();
      }

      return new CommandArgsItem(oItemType, name, value);
    }
  }
}
