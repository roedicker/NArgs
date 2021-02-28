using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NArgs.Attributes;
using NArgs.Models;

using NExtents;

namespace NArgs.Services
{
  /// <summary>
  /// Defines the default property service for parsing arguments.
  /// </summary>
  public sealed class DefaultPropertyService : IPropertyService
  {
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
    /// Initializes a new instance of the default property service.
    /// </summary>
    /// <param name="configuration">Configuration attached to this property service.</param>
    internal DefaultPropertyService(object configuration)
    {
      _AssignedOptions = new List<Option>();
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      Options = new ParseOptions();
      CustomDataTypeHandlers = new Dictionary<Type, CustomDataTypeHandler>();
    }

    /// <summary>
    /// Initializes a new instance of the default property service with custom parse options.
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
      return Configuration.GetType().GetProperties();
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
        if (property.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
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
        if (property.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
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
    public bool IsRequired(PropertyInfo property)
    {
      if (property == null)
      {
        throw new ArgumentNullException(nameof(property));
      }

      if (property.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
      {
        return option.Required;
      }
      else
      {
        return false;
      }
    }

    /// <inheritdoc />
    public string GetOptionName(PropertyInfo property)
    {
      if (property == null)
      {
        throw new ArgumentNullException(nameof(property));
      }

      if (property.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
      {
        return option.Name ?? option.AlternativeName ?? option.LongName ?? string.Empty;
      }
      else
      {
        throw new ArgumentException(Resources.PropertyDoesNotHaveAnOptionAttributeErrorMessage, nameof(property));
      }
    }

    /// <inheritdoc />
    public IEnumerable<string> GetUnassignedRequiredOptionNames()
    {
      return GetProperties()
             .Where(p => p.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option &&
                         (option.Required &&
                         !_AssignedOptions.Contains(option)))
             .Select(p => (p.GetCustomAttributes(typeof(Option), true).FirstOrDefault() as Option)?.Name ?? Resources.NotApplicableValue);
    }

    /// <inheritdoc />
    public void SetPropertyValue(PropertyInfo property, string? value)
    {
      if (property == null)
      {
        throw new ArgumentNullException(nameof(property));
      }

      // if argument has no value, property has to be of type "bool" - otherwise check type against given value
      if (string.IsNullOrWhiteSpace(value))
      {
        if (property.PropertyType.FullName == PropertyTypeFullName.Boolean)
        {
          property.SetValue(Configuration, true);
        }
        else
        {
          throw new ArgumentException(Resources.PropertyWithoutValueRequiresTypeBooleanErrorMessage, property.Name);
        }
      }
      else
      {
        switch (property.PropertyType.FullName)
        {
          case PropertyTypeFullName.String:
            property.SetValue(Configuration, value);
            break;

          case PropertyTypeFullName.Boolean:

            if (IsBooleanTrueValue(value))
            {
              property.SetValue(Configuration, true);
            }
            else if (IsBooleanFalseValue(value))
            {
              property.SetValue(Configuration, false);
            }
            break;

          case PropertyTypeFullName.Char:
            property.SetValue(Configuration, char.Parse(value));
            break;

          case PropertyTypeFullName.DateTime:
            property.SetValue(Configuration, DateTime.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Double:
            property.SetValue(Configuration, double.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Int16:
            property.SetValue(Configuration, short.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Int32:
            property.SetValue(Configuration, int.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Int64:
            property.SetValue(Configuration, long.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.FileInfo:
            property.SetValue(Configuration, new FileInfo(value));
            break;

          case PropertyTypeFullName.DirectoryInfo:
            property.SetValue(Configuration, new DirectoryInfo(value));
            break;

          case PropertyTypeFullName.Single:
            property.SetValue(Configuration, float.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.UInt16:
            property.SetValue(Configuration, ushort.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.UInt32:
            property.SetValue(Configuration, uint.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.UInt64:
            property.SetValue(Configuration, ulong.Parse(value, Options.Culture));
            break;

          case PropertyTypeFullName.Uri:
            property.SetValue(Configuration, new Uri(value));
            break;

          default:
            property.SetValue(Configuration, GetCustomDataTypeValue(property.PropertyType.GetType(), property.Name, value));
            break;
        }

        // set "assigned" flag if property is an option
        if(property.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
        {
          if(!_AssignedOptions.Contains(option))
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
    public bool IsValidValue(PropertyInfo property, string? value)
    {
      if (property == null)
      {
        throw new ArgumentNullException(nameof(property));
      }

      bool result;
      var isValueRequired = IsRequired(property);

      // if args has no value, property has to be of type "bool" - otherwise check type against given value
      if (value == null || string.IsNullOrWhiteSpace(value)) // just for the false positive CS8604
      {
        if (property.PropertyType.FullName == PropertyTypeFullName.Boolean)
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
        switch (property.PropertyType.FullName)
        {
          case PropertyTypeFullName.Char:
            result = char.TryParse(value, out char _);
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
            result = DateTime.TryParse(value, out DateTime _);
            break;

          case PropertyTypeFullName.DirectoryInfo:
            result = FileSystemInfo.IsValidDirectoryName(value);

            if (result == true && isValueRequired)
            {
              result = Directory.Exists(value);
            }
            break;

          case PropertyTypeFullName.Double:
            result = double.TryParse(value, out double _);
            break;

          case PropertyTypeFullName.Int16:
            result = short.TryParse(value, out short _);
            break;

          case PropertyTypeFullName.Int32:
            result = int.TryParse(value, out int _);
            break;

          case PropertyTypeFullName.Int64:
            result = long.TryParse(value, out long _);
            break;

          case PropertyTypeFullName.FileInfo:
            result = FileSystemInfo.IsValidFileName(value);

            if (result == true && isValueRequired)
            {
              result = File.Exists(value);
            }
            break;

          case PropertyTypeFullName.Single:
            result = float.TryParse(value, out float _);
            break;

          case PropertyTypeFullName.UInt16:
            result = ushort.TryParse(value, out ushort _);
            break;

          case PropertyTypeFullName.UInt32:
            result = uint.TryParse(value, out uint _);
            break;

          case PropertyTypeFullName.UInt64:
            result = ulong.TryParse(value, out ulong _);
            break;

          case PropertyTypeFullName.Uri:
            result = Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute);
            break;

          default:
            result = IsValidCustomDataTypeValue(property.PropertyType.GetType(), property.Name, value, IsRequired(property));
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
        if (property.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option _)
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
        CustomDataTypeHandler oHandlerSet = CustomDataTypeHandlers[type];

        result = oHandlerSet.Validator(name, value, required);
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

    private List<Option> _AssignedOptions;
  }
}
