using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;

using StringCollection = NCollections.StringCollection;

using NArgs.Attributes;
using NArgs.Exceptions;
using NArgs.Models;

namespace NArgs
{
  /// <summary>
  /// Defines the base command line parser
  /// </summary>
  public abstract class CommandLineParser
  {
    /// <summary>
    /// Gets or sets the property service
    /// </summary>
    protected IPropertyService PropertyService
    {
      get;
    }

    /// <summary>
    /// Gets or sets the tokenizer
    /// </summary>
    protected ICommandTokenizer Tokenizer
    {
      get;
    }

    /// <summary>
    /// Creates a new instance of the base command line parser
    /// </summary>
    /// <param name="propertyService">Property service of the command line parse</param>
    /// <param name="tokenizer">Command argument tokenizer</param>
    protected CommandLineParser(IPropertyService propertyService, ICommandTokenizer tokenizer)
    {
      this.PropertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
      this.Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
    }

    /// <summary>
    /// Parses the command line arguments
    /// </summary>
    /// <returns>Parse result</returns>
    internal ParseResult ParseCommandLine()
    {
      ParseResult Result = new ParseResult();
      int iParameterNumber = 1;

      try
      {
        // check plausibility: Options must follow parameters 
        bool bOptionsAvailable = false;

        foreach (CommandArgsItem oItem in this.Tokenizer.Items)
        {
          if (oItem.ItemType == CommandArgsItemType.Parameter)
          {
            if (bOptionsAvailable)
            {
              Result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat, oItem.Name, oItem.Value, Resources.ParametersMustPrecedeAnyOptionsErrorMessage));
            }
          }
          else
          {
            bOptionsAvailable = true;
          }
        }

        // process parameters
        foreach (CommandArgsItem oItem in this.Tokenizer.Items)
        {
          try
          {
            foreach (System.Reflection.PropertyInfo oInfo in this.PropertyService.GetProperties())
            {
              if (oInfo.GetCustomAttributes(typeof(Parameter), true).FirstOrDefault() is Parameter oParameter && oParameter.OrdinalNumber == iParameterNumber)
              {
                if (oItem.ItemType == CommandArgsItemType.Parameter)
                {
                  this.PropertyService.SetPropertyValue(oInfo, oItem.Name);
                  iParameterNumber++;
                  break;
                }
              }
            }
          }
          catch (Exception ex)
          {
            Result.AddError(new ParseError(ParseErrorType.UnknownError, oItem.Name, oItem.Value, ex.Message));
          }
        }

        // process options
        foreach (System.Reflection.PropertyInfo oInfo in this.PropertyService.GetProperties())
        {
          try
          {
            bool bOptionProcessed = false;

            if (oInfo.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option oOption)
            {
              foreach (CommandArgsItem oItem in this.Tokenizer.Items)
              {
                switch (oItem.ItemType)
                {
                  case CommandArgsItemType.Option:

                    if (oOption.Name == oItem.Name ||
                        oOption.LongName == oItem.Name ||
                        oOption.AlternativeName == oItem.Name)
                    {
                      if (this.PropertyService.IsValidOptionValue(oItem.Name, oItem.Value))
                      {
                        this.PropertyService.SetPropertyValue(oInfo, oItem.Value);
                        bOptionProcessed = true;
                      }
                      else
                      {
                        Result.AddError(new ParseError(ParseErrorType.InvalidOptionValue, oItem.Name, oItem.Value, String.Format(CultureInfo.InvariantCulture, Resources.ValueIsInvalidForOptionFormatErrorMessage, oItem.Value, oItem.Name)));
                      }
                    }

                    break;

                  case CommandArgsItemType.OptionLongName:

                    if (oOption.LongName == oItem.Name)
                    {
                      if (this.PropertyService.IsValidOptionValue(oItem.Name, oItem.Value))
                      {
                        this.PropertyService.SetPropertyValue(oInfo, oItem.Value);
                        bOptionProcessed = true;
                      }
                      else
                      {
                        Result.AddError(new ParseError(ParseErrorType.InvalidOptionValue, oItem.Name, oItem.Value, String.Format(CultureInfo.InvariantCulture, Resources.ValueIsInvalidForOptionFormatErrorMessage, oItem.Value, oItem.Name)));
                      }
                    }

                    break;
                }

                if (bOptionProcessed)
                {
                  break;
                }
              }
            }

            if (!bOptionProcessed && this.PropertyService.IsRequired(oInfo))
            {
              string sOptionName = this.PropertyService.GetOptionName(oInfo);

              Result.AddError(new ParseError(ParseErrorType.RequiredOptionValue, sOptionName, null, String.Format(CultureInfo.InvariantCulture, Resources.OptionIsMissingRequiredValueFormatErrorMessage, sOptionName)));
            }
          }
          catch (Exception ex)
          {
            Result.AddError(new ParseError(ParseErrorType.UnknownError, Resources.NotAvailableShortName, null, ex.Message));
          }
        }
      }
      catch (InvalidCommandArgsFormatException ex)
      {
        Result.AddError(new ParseError(ParseErrorType.InvalidCommandArgsFormat, ex.ItemName, null, ex.Message));
      }
      catch (Exception ex)
      {
        Result.AddError(new ParseError(ParseErrorType.UnknownError, Resources.NotAvailableShortName, null, ex.Message));
      }

      return Result;
    }

    /// <summary>
    /// Gets the usage for the current configuration
    /// </summary>
    /// <param name="executable">Current executable name</param>
    /// <returns>Usage output for the current configuration</returns>
    internal string GetUsage(string executable)
    {
      StringBuilder Result = new StringBuilder();
      List<Parameter> lstParameters = new List<Parameter>();
      StringCollection colParametersSyntaxUsage = new StringCollection();
      StringDictionary dicOptions = new StringDictionary();
      StringCollection colOptionsSyntaxUsage = new StringCollection();
      int iMaxParameterNameLength = 0;
      int iMaxOptionCompleteNameLength = 0;

      foreach (System.Reflection.PropertyInfo oInfo in this.PropertyService.GetProperties())
      {
        if (oInfo.GetCustomAttributes(typeof(Parameter), true).FirstOrDefault() is Parameter oParameter)
        {
          lstParameters.Add(oParameter);

          if (oParameter.Name?.Length > iMaxParameterNameLength)
          {
            iMaxParameterNameLength = (int)oParameter.Name.Length;
          }
        }
        else
        {
          if (oInfo.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option oOption)
          {
            StringCollection colOptionNames = new StringCollection();

            if (!String.IsNullOrWhiteSpace(oOption.Name))
            {
              colOptionNames.AddDistinct($"{this.Tokenizer.ArgumentOptionDefaultNameIndicator}{oOption.Name}");
            }

            if (!String.IsNullOrWhiteSpace(oOption.AlternativeName))
            {
              colOptionNames.AddDistinct($"{this.Tokenizer.ArgumentOptionDefaultNameIndicator}{oOption.AlternativeName}");
            }

            if (!String.IsNullOrWhiteSpace(oOption.LongName))
            {
              colOptionNames.AddDistinct($"{this.Tokenizer.ArgumentOptionLongNameIndicator}{oOption.LongName}");
            }

            string sOptionCompleteName = colOptionNames.ToString(" | ");

            if (oOption.Required)
            {
              if (oInfo.PropertyType.FullName == PropertyTypeFullName.Boolean)
              {
                colOptionsSyntaxUsage.Add($"{sOptionCompleteName}");
              }
              else
              {
                colOptionsSyntaxUsage.Add($"{sOptionCompleteName} <{oOption.UsageTypeDisplayName}>");
              }
            }
            else
            {
              if (oInfo.PropertyType.FullName == PropertyTypeFullName.Boolean)
              {
                colOptionsSyntaxUsage.Add($"[{sOptionCompleteName}]");
              }
              else
              {
                colOptionsSyntaxUsage.Add($"[{sOptionCompleteName} <{oOption.UsageTypeDisplayName}>]");
              }
            }

            dicOptions.Add(sOptionCompleteName, String.IsNullOrWhiteSpace(oOption.Description) ? Resources.NotApplicableValue : oOption.Description);

            if (sOptionCompleteName.Length > iMaxOptionCompleteNameLength)
            {
              iMaxOptionCompleteNameLength = sOptionCompleteName.Length;
            }
          }
        }
      }

      // sort parameters by their ordinal numbers
      lstParameters.Sort((l, r) => l.OrdinalNumber.CompareTo(r.OrdinalNumber));

      Result.AppendLine($"{Resources.SyntaxCapitalizedName}:");
      Result.Append($"  {executable} ");

      if (lstParameters.Count > 0)
      {
        foreach (Parameter oParameter in lstParameters)
        {
          Result.AppendFormat(CultureInfo.InvariantCulture, "<{0}> ", String.IsNullOrWhiteSpace(oParameter.Name) ? Resources.NotApplicableValue : oParameter.Name);
        }
      }

      StringBuilder sbIndention = new StringBuilder();

      for (int i = 0; i < executable.Length + 3; i++)
      {
        sbIndention.Append(' ');
      }

      Result.Append($"{colOptionsSyntaxUsage.ToString($"{Environment.NewLine}{sbIndention}")}{Environment.NewLine}");

      if (lstParameters.Count > 0)
      {
        Result.AppendLine($"{Environment.NewLine}{Resources.ParametersCapitalizedName}:");

        foreach (Parameter oParameter in lstParameters)
        {
          Result.AppendFormat(CultureInfo.InvariantCulture, "  {0, " + Convert.ToString(-iMaxParameterNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}", String.IsNullOrWhiteSpace(oParameter.Name) ? "n/a" : oParameter.Name, String.IsNullOrWhiteSpace(oParameter.Description) ? "n/a" : oParameter.Description, Environment.NewLine);
        }
      }

      if (dicOptions.Count > 0)
      {
        Result.AppendLine($"{Environment.NewLine}{Resources.OptionsCapitalizedName}:");

        foreach (string sKey in dicOptions.Keys)
        {
          Result.AppendFormat(CultureInfo.InvariantCulture, "  {0, " + Convert.ToString(-iMaxOptionCompleteNameLength, CultureInfo.InvariantCulture) + "}     {1}{2}", sKey, dicOptions[sKey], Environment.NewLine);
        }
      }

      return Result.ToString();
    }
  }
}
