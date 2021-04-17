using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

using NArgs.Attributes;
using NArgs.Extensions;
using NArgs.Models;

using NExtents;

namespace NArgs.Services
{
  /// <summary>
  /// Defines the default property service for parsing arguments.
  /// </summary>
  public sealed class DefaultPropertyService : IPropertyService
  {
    private readonly List<OptionAttribute> _AssignedOptions;
    private object? _CurrentCommandConfiguration;

    /// <summary>
    /// Gets the parse options.
    /// </summary>
    public ParseOptions Options
    {
      get;
    }

    /// <summary>
    /// Gets the configuration used for argument parsing.
    /// </summary>
    private object Configuration
    {
      get;
    }

    /// <summary>
    /// Gets the dictionary of registered custom data-type handlers.
    /// </summary>
    private Dictionary<Type, CustomDataTypeHandler> CustomDataTypeHandlers
    {
      get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultPropertyService" /> class.
    /// </summary>
    /// <param name="configuration">Configuration attached to this property service.</param>
    internal DefaultPropertyService(object configuration)
    {
      _AssignedOptions = new List<OptionAttribute>();
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      Options = new ParseOptions();
      CustomDataTypeHandlers = new Dictionary<Type, CustomDataTypeHandler>();

      ValidateConfiguration();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultPropertyService" /> class with custom parse options.
    /// </summary>
    /// <param name="configuration">Configuration attached to this property service.</param>
    /// <param name="options"></param>
    internal DefaultPropertyService(object configuration, ParseOptions options) : this(configuration)
    {
      Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public IEnumerable<PropertyInfo> GetProperties()
    {
      return GetCurrentConfiguration()
             .GetType()
             .GetProperties();
    }

    /// <inheritdoc />
    public IEnumerable<PropertyInfo> GetGlobalProperties()
    {
      return Configuration
             .GetType()
             .GetProperties();
    }

    /// <inheritdoc />
    public bool HasCommands()
    {
      return GetProperties().Any(prop => prop.IsCommand());
    }

    /// <inheritdoc />
    public bool HasOptions()
    {
      return GetProperties().Any(prop => prop.IsOption());
    }

    /// <inheritdoc />
    public bool HasParameters()
    {
      return GetProperties().Any(prop => prop.IsParameter());
    }

    /// <inheritdoc />
    public Type GetPropertyTypeByOptionName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(name));
      }

      foreach (var property in GetProperties())
      {
        if (property.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() is OptionAttribute option)
        {
          if (option.Name == name || option.AlternativeName == name || option.LongName == name)
          {
            return property.PropertyType;
          }
        }
      }

      throw new ArgumentException(Resources.OptionDoesNotExistErrorMessage, nameof(name));
    }

    /// <inheritdoc />
    public string GetPropertyTypeNameByOptionName(string name)
    {
      return GetPropertyTypeByOptionName(name).ToString();
    }

    /// <inheritdoc />
    public PropertyInfo GetPropertyByOptionName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(name));
      }

      foreach (var property in GetProperties())
      {
        if (property.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() is OptionAttribute option)
        {
          if (option.Name == name || option.AlternativeName == name || option.LongName == name)
          {
            return property;
          }
        }
      }

      throw new ArgumentException(Resources.OptionDoesNotExistErrorMessage, name);
    }

    /// <inheritdoc />
    public PropertyInfo GetPropertyByCommandName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(name));
      }

      foreach (var property in GetGlobalProperties())
      {
        if (property.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault() is CommandAttribute command)
        {
          if (command.Name == name)
          {
            return property;
          }
        }
      }

      throw new ArgumentException(Resources.CommandDoesNotExistErrorMessage, name);
    }

    /// <inheritdoc />
    public bool IsRequired(PropertyInfo prop)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      if (prop.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() is OptionAttribute option)
      {
        return option.Required;
      }
      else
      {
        return false;
      }
    }

    /// <inheritdoc />
    public string GetOptionName(PropertyInfo prop)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      var option = GetOption(prop);

      return option.Name ?? option.AlternativeName ?? option.LongName ?? string.Empty;
    }

    /// <inheritdoc />
    public OptionAttribute GetOption(PropertyInfo prop)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      return prop.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() as OptionAttribute ??
             throw new ArgumentException(Resources.PropertyDoesNotHaveAnOptionAttributeErrorMessage, nameof(prop));
    }

    /// <inheritdoc />
    public ParameterAttribute GetParameter(PropertyInfo prop)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      return prop.GetCustomAttributes(typeof(ParameterAttribute), true).FirstOrDefault() as ParameterAttribute ??
             throw new ArgumentException(Resources.PropertyDoesNotHaveAParameterAttributeErrorMessage, nameof(prop));
    }

    /// <inheritdoc />
    public CommandAttribute GetCommand(PropertyInfo prop)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      return prop.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault() as CommandAttribute ??
             throw new ArgumentException(Resources.PropertyDoesNotHaveACommandAttributeErrorMessage, nameof(prop));
    }

    /// <inheritdoc />
    public IEnumerable<string> GetUnassignedRequiredOptionNames()
    {
      return GetProperties()
             .Where(p => p.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() is OptionAttribute option &&
                         option.Required &&
                         !_AssignedOptions.Contains(option))
             .Select(p => (p.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() as OptionAttribute)?.Name ?? Resources.NotApplicableValue);
    }

    /// <inheritdoc />
    public object GetPropertyValue(PropertyInfo prop)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      return prop.GetValue(GetCurrentConfiguration());
    }

    /// <inheritdoc />
    public object GetGlobalPropertyValue(PropertyInfo prop)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      return prop.GetValue(Configuration);
    }

    /// <inheritdoc />
    public void SetCurrentCommand(object command)
    {
      _CurrentCommandConfiguration = command ?? throw new ArgumentNullException(nameof(command));
    }

    /// <inheritdoc />
    public void ResetCurrentCommand()
    {
      _CurrentCommandConfiguration = null;
    }

    /// <inheritdoc />
    public void SetPropertyValue(PropertyInfo prop, string? value)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      // if argument has no value, property has to be of type "bool" - otherwise check type against given value
      if (string.IsNullOrWhiteSpace(value))
      {
        if (prop.PropertyType.FullName == PropertyTypeFullName.Boolean)
        {
          prop.SetValue(GetCurrentConfiguration(), true);
        }
        else
        {
          throw new ArgumentException(Resources.PropertyWithoutValueRequiresTypeBooleanErrorMessage, prop.Name);
        }
      }
      else
      {
        switch (prop.PropertyType.FullName)
        {
          case PropertyTypeFullName.String:
            prop.SetValue(GetCurrentConfiguration(), value);
            break;

          case PropertyTypeFullName.Boolean:

            if (IsBooleanTrueValue(value))
            {
              prop.SetValue(GetCurrentConfiguration(), true);
            }
            else if (IsBooleanFalseValue(value))
            {
              prop.SetValue(GetCurrentConfiguration(), false);
            }
            break;

          case PropertyTypeFullName.Char:
            prop.SetValue(GetCurrentConfiguration(), char.Parse(value));
            break;

          case PropertyTypeFullName.DateTime:
            prop.SetValue(GetCurrentConfiguration(), DateTime.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Double:
            prop.SetValue(GetCurrentConfiguration(), double.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Int16:
            prop.SetValue(GetCurrentConfiguration(), short.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Int32:
            prop.SetValue(GetCurrentConfiguration(), int.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Int64:
            prop.SetValue(GetCurrentConfiguration(), long.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.FileInfo:
            prop.SetValue(GetCurrentConfiguration(), new FileInfo(value));
            break;

          case PropertyTypeFullName.DirectoryInfo:
            prop.SetValue(GetCurrentConfiguration(), new DirectoryInfo(value));
            break;

          case PropertyTypeFullName.Single:
            prop.SetValue(GetCurrentConfiguration(), float.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.UInt16:
            prop.SetValue(GetCurrentConfiguration(), ushort.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.UInt32:
            prop.SetValue(GetCurrentConfiguration(), uint.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.UInt64:
            prop.SetValue(GetCurrentConfiguration(), ulong.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Uri:
            prop.SetValue(GetCurrentConfiguration(), new Uri(value));
            break;

          default:
            prop.SetValue(GetCurrentConfiguration(), GetCustomDataTypeValue(prop.PropertyType.GetType(), prop.Name, value));
            break;
        }

        // set "assigned" flag if property is an option
        if (prop.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() is OptionAttribute option)
        {
          if (!_AssignedOptions.Contains(option))
          {
            _AssignedOptions.Add(option);
          }
        }
      }
    }

    /// <inheritdoc />
    public void AddCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter getter, PropertyServiceCustomDataTypeValidator validator)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      if (getter == null)
      {
        throw new ArgumentNullException(nameof(getter));
      }

      if (validator == null)
      {
        throw new ArgumentNullException(nameof(validator));
      }

      CustomDataTypeHandlers.Add(type.GetType(), new CustomDataTypeHandler(getter, validator));
    }

    /// <inheritdoc />
    public bool IsValidValue(PropertyInfo prop, string? value)
    {
      if (prop == null)
      {
        throw new ArgumentNullException(nameof(prop));
      }

      bool result;
      var isValueRequired = IsRequired(prop);

      // if args has no value, property has to be of type "bool" - otherwise check type against given value
      if (value == null || string.IsNullOrWhiteSpace(value)) // just for the false positive CS8604
      {
        if (prop.PropertyType.FullName == PropertyTypeFullName.Boolean)
        {
          result = true;
        }
        else
        {
          result = false;
        }
      }
      else
      {
        switch (prop.PropertyType.FullName)
        {
          case PropertyTypeFullName.Char:
            result = char.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.String:
            result = true;
            break;

          case PropertyTypeFullName.Boolean:

            if (IsBooleanTrueValue(value) || IsBooleanFalseValue(value))
            {
              result = true;
            }
            else
            {
              result = false;
            }
            break;

          case PropertyTypeFullName.DateTime:
            result = DateTime.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.DirectoryInfo:
            result = FileSystemInfo.IsValidDirectoryName(value);

            if (result == true && isValueRequired)
            {
              result = Directory.Exists(value);
            }
            break;

          case PropertyTypeFullName.Double:
            result = double.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.Int16:
            result = short.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.Int32:
            result = int.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.Int64:
            result = long.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.FileInfo:
            result = FileSystemInfo.IsValidFileName(value);

            if (result == true && isValueRequired)
            {
              result = File.Exists(value);
            }
            break;

          case PropertyTypeFullName.Single:
            result = float.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.UInt16:
            result = ushort.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.UInt32:
            result = uint.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.UInt64:
            result = ulong.TryParse(value, out var _);
            break;

          case PropertyTypeFullName.Uri:
            result = Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute);
            break;

          default:
            result = IsValidCustomDataTypeValue(prop.PropertyType.GetType(), prop.Name, value, IsRequired(prop));
            break;
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool IsValidOptionValue(string name, string? value)
    {
      var property = GetPropertyByOptionName(name);

      if (property == null)
      {
        return false;
      }
      else
      {
        if (property.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() is OptionAttribute _)
        {
          return IsValidValue(property, value);
        }
        else
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Gets the current configuration.
    /// </summary>
    /// <returns>Current configuration (e.g. current command or initial configuration.</returns>
    private object GetCurrentConfiguration()
    {
      return _CurrentCommandConfiguration ?? Configuration;
    }

    /// <summary>
    /// Gets an indicator whether a given value represents a boolean false-value or not.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns><see langword="true" /> if given value represents a boolean false-value, otherwise <see langword="false" />.</returns>
    private static bool IsBooleanFalseValue(string? value)
    {
      return new string[] { "n", "no", "false", "off", "0" }.Contains(value, StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Gets an indicator whether a given value represents a boolean true-value or not.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns><see langword="true" /> if given value represents a boolean true-value, otherwise <see langword="false" />.</returns>
    private static bool IsBooleanTrueValue(string? value)
    {
      return new string[] { "y", "yes", "true", "on", "1" }.Contains(value, StringComparer.InvariantCultureIgnoreCase);
    }


    /// <summary>
    /// Gets an indication whether a command value is valid for a custom data-type or not.
    /// </summary>
    /// <param name="type">Custom data-type.</param>
    /// <param name="name">Property name.</param>
    /// <param name="value">Command value.</param>
    /// <param name="required">Indicator whether that value has to be required or not.</param>
    /// <returns><see langword="true" /> if custom data-ytpe is valid, otherwise <see langword="false" />.</returns>
    private bool IsValidCustomDataTypeValue(Type type, string name, string? value, bool required)
    {
      var result = false;

      if (CustomDataTypeHandlers.ContainsKey(type))
      {
        var handlers = CustomDataTypeHandlers[type];

        result = handlers.Validator(name, value, required);
      }

      return result;
    }

    /// <summary>
    /// Gets the value of a custom data-type based on a command value.
    /// </summary>
    /// <param name="type">Custom data-type.</param>
    /// <param name="name">Property name.</param>
    /// <param name="value">Command value of the property.</param>
    /// <returns>Value of custom data-type based on a command value.</returns>
    private object? GetCustomDataTypeValue(Type type, string name, string? value)
    {
      object? Result = null;

      if (CustomDataTypeHandlers.ContainsKey(type))
      {
        var handler = CustomDataTypeHandlers[type];

        Result = handler.Getter(name, value);
      }

      return Result;
    }

    /// <summary>
    /// Validates the referenced configuration.
    /// </summary>
    private void ValidateConfiguration()
    {
      var optionNames = new List<string>();
      var parameterNames = new List<string>();
      var parameterOrdinals = new List<uint>();

      foreach (var prop in GetProperties())
      {
        if (prop.GetOption() is OptionAttribute option)
        {
          ValidateOptionName(prop, optionNames, option);
          ValidateOptionAlternativeName(optionNames, option);
          ValidateOptionLongName(optionNames, option);
        }
        else if (prop.GetParameter() is ParameterAttribute parameter)
        {
          ValidateParameter(parameter);
          ValidateParameterName(parameterNames, parameterOrdinals, parameter);
        }
        else if (prop.GetCommand() is CommandAttribute command)
        {
          var commandParameterNames = new List<string>();
          var commandParameterOrdinals = new List<uint>();

          _CurrentCommandConfiguration = command;

          try
          {
            // validate properties of current command
            foreach (var cmdProp in prop.PropertyType.GetProperties())
            {
              if (cmdProp.GetOption() is OptionAttribute cmdOption)
              {
                ValidateOptionName(cmdProp, optionNames, cmdOption);
                ValidateOptionAlternativeName(optionNames, cmdOption);
                ValidateOptionLongName(optionNames, cmdOption);
              }
              else if (cmdProp.GetParameter() is ParameterAttribute cmdParameter)
              {
                ValidateParameter(cmdParameter);
                ValidateParameterName(commandParameterNames, commandParameterOrdinals, cmdParameter);
              }
              else if (cmdProp.GetCommand() is CommandAttribute cmdCommand)
              {
                throw new InvalidConfigurationException(string.Format(CultureInfo.InvariantCulture,
                                                                      Resources.NestedCommandsAreNotAllowed,
                                                                      cmdProp.Name));
              }
            }
          }
          finally
          {
            _CurrentCommandConfiguration = null;
          }

          ValidateParameterOrdinals(commandParameterOrdinals);
        }
      }

      ValidateParameterOrdinals(parameterOrdinals);
    }

    /// <summary>
    /// Validates a name of an option.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <param name="optionNames">Collection of used option names.</param>
    /// <param name="option">Option to validate.</param>
    private static void ValidateOptionName(PropertyInfo prop, IList<string> optionNames, OptionAttribute option)
    {
      // check for required and duplicate option names
      if (string.IsNullOrWhiteSpace(option.Name))
      {
        throw new InvalidConfigurationException(string.Format(CultureInfo.InvariantCulture,
                                                              Resources.OptionIsMissingRequiredNameFormatErrorMessage,
                                                              prop.Name));
      }

      if (optionNames.Contains(option.Name.ToUpperInvariant()))
      {
        throw new InvalidConfigurationException(string.Format(CultureInfo.InvariantCulture,
                                                              Resources.OptionNameAlreadyUsedFormatErrorMessage,
                                                              option.Name));
      }
      else
      {
        optionNames.Add(option.Name.ToUpperInvariant());
      }
    }

    /// <summary>
    /// Validates a alternative name of an option.
    /// </summary>
    /// <param name="optionNames">Collection of used option names.</param>
    /// <param name="option">Option to validate.</param>
    private static void ValidateOptionAlternativeName(IList<string> optionNames, OptionAttribute option)
    {
      if (!string.IsNullOrWhiteSpace(option.AlternativeName))
      {
        if (optionNames.Contains(option.AlternativeName.ToUpperInvariant()))
        {
          throw new InvalidConfigurationException(string.Format(CultureInfo.InvariantCulture,
                                                                Resources.OptionAlternativeNameAlreadyUsedFormatErrorMessage,
                                                                option.AlternativeName));
        }
        else
        {
          optionNames.Add(option.AlternativeName.ToUpperInvariant());
        }
      }
    }

    /// <summary>
    /// Validates a long name of an option.
    /// </summary>
    /// <param name="optionNames">Collection of used option names.</param>
    /// <param name="option">Option to validate.</param>
    private static void ValidateOptionLongName(IList<string> optionNames, OptionAttribute option)
    {
      if (!string.IsNullOrWhiteSpace(option.LongName))
      {
        if (optionNames.Contains(option.LongName.ToUpperInvariant()))
        {
          throw new InvalidConfigurationException(string.Format(CultureInfo.InvariantCulture,
                                                                Resources.OptionLongNameAlreadyUsedFormatErrorMessage,
                                                                option.LongName));
        }
        else
        {
          optionNames.Add(option.LongName.ToUpperInvariant());
        }
      }
    }

    /// <summary>
    /// Validates a parameter.
    /// </summary>
    /// <param name="parameter">Parameter to validate.</param>
    private void ValidateParameter(ParameterAttribute parameter)
    {
      if (HasCommands())
      {
        throw new InvalidConfigurationException(string.Format(CultureInfo.InvariantCulture,
                                                              Resources.ParametersNotAllowedToCombineWithCommands,
                                                              parameter.Name));
      }
    }

    /// <summary>
    /// Validates a name of a parameter.
    /// </summary>
    /// <param name="parameterNames">Collection of used parameter names.</param>
    /// <param name="parameterOrdinals">Collection of used parameter ordinals.</param>
    /// <param name="parameter">Parameter to validate.</param>
    private static void ValidateParameterName(IList<string> parameterNames, IList<uint> parameterOrdinals, ParameterAttribute parameter)
    {
      // check for duplicate parameter names
      if (!string.IsNullOrWhiteSpace(parameter.Name))
      {
        if (parameterNames.Contains(parameter.Name.ToUpperInvariant()))
        {
          throw new InvalidConfigurationException(string.Format(CultureInfo.InvariantCulture,
                                                                Resources.ParameterNameAlreadyUsedFormatErrorMessage,
                                                                parameter.Name));
        }
        else
        {
          parameterNames.Add(parameter.Name.ToUpperInvariant());
        }
      }

      // check for duplicate parameter ordinals
      if (parameterOrdinals.Contains(parameter.OrdinalNumber))
      {
        throw new InvalidConfigurationException(string.Format(CultureInfo.InvariantCulture,
                                                              Resources.ParameterOrdinalAlreadyUsedFormatErrorMessage,
                                                              parameter.OrdinalNumber));
      }
      else
      {
        parameterOrdinals.Add(parameter.OrdinalNumber);
      }
    }

    /// <summary>
    /// Validates the parameter ordinals.
    /// </summary>
    /// <param name="parameterOrdinals">Collection of used parameter ordinals.</param>
    private static void ValidateParameterOrdinals(List<uint> parameterOrdinals)
    {
      if (parameterOrdinals.Any())
      {
        // check for correct ordinal sequence (starting at #1)
        parameterOrdinals.Sort();

        if (parameterOrdinals.First() != 1 ||
            parameterOrdinals.Last() != parameterOrdinals.Count)
        {
          throw new InvalidConfigurationException(Resources.ParameterOrdinalNotUsedInSequenceErrorMessage);
        }
      }
    }

    /// <summary>
    /// Defines a custom data-type handler.
    /// </summary>
    private class CustomDataTypeHandler
    {
      /// <summary>
      /// Gets or sets the property getter of a custom data-type handler.
      /// </summary>
      public PropertyServiceCustomDataTypeGetter Getter
      {
        get;
        set;
      }

      /// <summary>
      /// Gets or sets the property validator of a custom data-type handler.
      /// </summary>
      public PropertyServiceCustomDataTypeValidator Validator
      {
        get;
        set;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CustomDataTypeHandler" /> class.
      /// </summary>
      /// <param name="getter">Getter for custom data-type property.</param>
      /// <param name="validator">validator for custom data-type property.</param>
      public CustomDataTypeHandler(PropertyServiceCustomDataTypeGetter getter, PropertyServiceCustomDataTypeValidator validator)
      {
        Getter = getter ?? throw new ArgumentNullException(nameof(getter));
        Validator = validator ?? throw new ArgumentNullException(nameof(validator));
      }
    }
  }
}
