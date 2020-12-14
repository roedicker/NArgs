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
  /// Defines the default property service for parsing arguments
  /// </summary>
  public sealed class DefaultPropertyService : IPropertyService
  {
    /// <summary>
    /// Gets the parse options
    /// </summary>
    public ParseOptions Options
    {
      get;
    }

    /// <summary>
    /// Gets the configuration used for argument parsing
    /// </summary>
    private object Config
    {
      get
      {
        if (_Config == null)
        {
          throw new InvalidOperationException(Resources.PropertyServiceNotInitializedErrorMessage);
        }

        return _Config;
      }
    }

    /// <summary>
    /// Gets the dictionary of registered custom data-type handlers
    /// </summary>
    private Dictionary<Type, CustomDataTypeHandler> CustomDataTypeHandlers
    {
      get;
    }

    /// <summary>
    /// Initializes a new instance of the default property service
    /// </summary>
    internal DefaultPropertyService()
    {
      this.Options = new ParseOptions();
      this.CustomDataTypeHandlers = new Dictionary<Type, CustomDataTypeHandler>();
    }

    /// <summary>
    /// Initializes a new instance of the default property service with custom parse options
    /// </summary>
    /// <param name="options"></param>
    internal DefaultPropertyService(ParseOptions options) : this()
    {
      this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Initializes the property service based on a given configuration
    /// </summary>
    /// <param name="config">Configuration to analyze</param>
    public void Init(object config)
    {
      _Config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Gets all properties
    /// </summary>
    /// <returns>All properties based on used configuration</returns>
    public IEnumerable<PropertyInfo> GetProperties()
    {
      return this.Config.GetType().GetProperties();
    }

    /// <summary>
    /// Gets the property type of an option by its name
    /// </summary>
    /// <param name="name">Option name</param>
    /// <returns>Property type of given option name</returns>
    /// <exception cref="ArgumentException">Option name is null or empty or option with given name does not exist</exception>
    public Type GetPropertyTypeByOptionName(string name)
    {
      if (String.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(name));
      }

      foreach (PropertyInfo oInfo in this.GetProperties())
      {
        if (oInfo.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option oOption)
        {
          if (oOption.Name == name || oOption.AlternativeName == name || oOption.LongName == name)
          {
            return oInfo.PropertyType;
          }
        }
      }

      throw new ArgumentException(Resources.OptionDoesNotExistErrorMessage, nameof(name));
    }

    /// <summary>
    /// Gets the property type-name of an option by its name
    /// </summary>
    /// <param name="name">Option name</param>
    /// <returns>Property type-name of given option name</returns>
    /// <exception cref="ArgumentException">Option name is null or empty or option with given name does not exist</exception>
    public string GetPropertyTypeNameByOptionName(string name)
    {
      return GetPropertyTypeByOptionName(name).ToString();
    }

    /// <summary>
    /// Gets the property information of an option by its name
    /// </summary>
    /// <param name="name">Option name</param>
    /// <returns>Property information of given option by its name</returns>
    public PropertyInfo GetPropertyByOptionName(string name)
    {
      if (String.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(name));
      }

      foreach (PropertyInfo oInfo in this.GetProperties())
      {
        if (oInfo.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option oOption)
        {
          if (oOption.Name == name || oOption.AlternativeName == name || oOption.LongName == name)
          {
            return oInfo;
          }
        }
      }

      throw new ArgumentException(Resources.OptionDoesNotExistErrorMessage, name);
    }

    /// <summary>
    /// Gets an indicator whether a property is stated as required or not
    /// </summary>
    /// <param name="info">Property information</param>
    /// <returns><c>true</c> if property is stated as required, otherwise <c>false</c>.</returns>
    public bool IsRequired(PropertyInfo info)
    {
      if (info == null)
      {
        throw new ArgumentNullException(nameof(info));
      }

      if (info.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option oOption)
      {
        return oOption.Required;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Gets the name of an option by its property information
    /// </summary>
    /// <param name="info">Property information of an option</param>
    /// <returns>Name of an option by its given property information</returns>
    public string GetOptionName(PropertyInfo info)
    {
      if (info == null)
      {
        throw new ArgumentNullException(nameof(info));
      }

      if (info.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option oOption)
      {
        return oOption.Name ?? oOption.AlternativeName ?? oOption.LongName ?? String.Empty;
      }
      else
      {
        throw new ArgumentException(Resources.PropertyDoesNotHaveAnOptionAttributeErrorMessage, nameof(info));
      }
    }

    /// <summary>
    /// Sets a value of a property
    /// </summary>
    /// <param name="property">Property information</param>
    /// <param name="value">Value to set</param>
    public void SetPropertyValue(PropertyInfo property, string value)
    {
      if (property == null)
      {
        throw new ArgumentNullException(nameof(property));
      }

      // if args has no value, property has to be of type "bool" - otherwise check type against given value
      if (String.IsNullOrWhiteSpace(value))
      {
        if (property.PropertyType.FullName == PropertyTypeFullName.Boolean)
        {
          property.SetValue(this.Config, true);
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
            property.SetValue(this.Config, value);
            break;

          case PropertyTypeFullName.Boolean:

            if (IsBooleanTrueValue(value))
            {
              property.SetValue(this.Config, true);
            }
            else if (IsBooleanFalseValue(value))
            {
              property.SetValue(this.Config, false);
            }
            break;

          case PropertyTypeFullName.Char:
            property.SetValue(this.Config, char.Parse(value));
            break;

          case PropertyTypeFullName.DateTime:
            property.SetValue(this.Config, DateTime.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.Double:
            property.SetValue(this.Config, double.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.Int16:
            property.SetValue(this.Config, short.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.Int32:
            property.SetValue(this.Config, int.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.Int64:
            property.SetValue(this.Config, long.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.FileInfo:
            property.SetValue(this.Config, new FileInfo(value));
            break;

          case PropertyTypeFullName.DirectoryInfo:
            property.SetValue(this.Config, new DirectoryInfo(value));
            break;

          case PropertyTypeFullName.Single:
            property.SetValue(this.Config, float.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.UInt16:
            property.SetValue(this.Config, ushort.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.UInt32:
            property.SetValue(this.Config, uint.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.UInt64:
            property.SetValue(this.Config, ulong.Parse(value, this.Options.Culture));
            break;

          case PropertyTypeFullName.Uri:
            property.SetValue(this.Config, new Uri(value));
            break;

          default:
            property.SetValue(this.Config, GetCustomDataTypeValue(property.PropertyType.GetType(), property.Name, value));
            break;
        }
      }
    }

    /// <summary>
    /// Adds a custom handler for a custom data-type
    /// </summary>
    /// <param name="type">Data type of the custom handler</param>
    /// <param name="getter">Getter of the custom handler</param>
    /// <param name="validator">Validator of the custom handler</param>
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

      this.CustomDataTypeHandlers.Add(type.GetType(), new CustomDataTypeHandler(getter, validator));
    }

    /// <summary>
    /// Checks whether a value is valid for a given property or not
    /// </summary>
    /// <param name="info">Property information</param>
    /// <param name="value">Value to validate</param>
    /// <returns><c>true</c> if value is valid, otherwise <c>false</c>.</returns>
    public bool IsValidValue(PropertyInfo info, string value)
    {
      if(info == null)
      {
        throw new ArgumentNullException(nameof(info));
      }

      bool Result;
      bool bIsValueRequired = IsRequired(info);

      // if args has no value, property has to be of type "bool" - otherwise check type against given value
      if (String.IsNullOrWhiteSpace(value))
      {
        if (info.PropertyType.FullName == PropertyTypeFullName.Boolean)
        {
          Result = true;
        }
        else
        {
          Result = false;
        }
      }
      else
      {
        switch (info.PropertyType.FullName)
        {
          case PropertyTypeFullName.Char:
            Result = char.TryParse(value, out char _);
            break;

          case PropertyTypeFullName.String:
            Result = true;
            break;

          case PropertyTypeFullName.Boolean:

            if (IsBooleanTrueValue(value) || IsBooleanFalseValue(value))
            {
              Result = true;
            }
            else
            {
              Result = false;
            }
            break;

          case PropertyTypeFullName.DateTime:
            Result = DateTime.TryParse(value, out DateTime _);
            break;

          case PropertyTypeFullName.DirectoryInfo:
            Result = FileSystemInfo.IsValidDirectoryName(value);

            if (Result == true && bIsValueRequired)
            {
              Result = Directory.Exists(value);
            }
            break;

          case PropertyTypeFullName.Double:
            Result = double.TryParse(value, out double _);
            break;

          case PropertyTypeFullName.Int16:
            Result = short.TryParse(value, out short _);
            break;

          case PropertyTypeFullName.Int32:
            Result = int.TryParse(value, out int _);
            break;

          case PropertyTypeFullName.Int64:
            Result = long.TryParse(value, out long _);
            break;

          case PropertyTypeFullName.FileInfo:
            Result = FileSystemInfo.IsValidFileName(value);

            if (Result == true && bIsValueRequired)
            {
              Result = File.Exists(value);
            }
            break;

          case PropertyTypeFullName.Single:
            Result = float.TryParse(value, out float _);
            break;

          case PropertyTypeFullName.UInt16:
            Result = ushort.TryParse(value, out ushort _);
            break;

          case PropertyTypeFullName.UInt32:
            Result = uint.TryParse(value, out uint _);
            break;

          case PropertyTypeFullName.UInt64:
            Result = ulong.TryParse(value, out ulong _);
            break;

          case PropertyTypeFullName.Uri:
            Result = Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute);
            break;

          default:
            Result = IsValidCustomDataTypeValue(info.PropertyType.GetType(), info.Name, value, IsRequired(info));
            break;
        }
      }

      return Result;
    }

    /// <summary>
    /// Gets an indicator whether an option value is valid or not
    /// </summary>
    /// <param name="name">Name of the option</param>
    /// <param name="value">Value of the option</param>
    /// <returns><c>true</c> if option value is valid, otherwise <c>false</c>.</returns>
    public bool IsValidOptionValue(string name, string value)
    {
      PropertyInfo oInfo = GetPropertyByOptionName(name);

      if (oInfo == null)
      {
        return false;
      }
      else
      {
        if (oInfo.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option _)
        {
          return this.IsValidValue(oInfo, value);
        }
        else
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Gets an indicator whether a given value represents a boolean false-value or not
    /// </summary>
    /// <param name="value">Value to validate</param>
    /// <returns><c>true</c> if given value represents a boolean false-value, otherwise <c>false</c>.</returns>
    private bool IsBooleanFalseValue(string value)
    {
      return new string[] { "n", "no", "false", "off", "0" }.Contains(value, StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Gets an indicator whether a given value represents a boolean true-value or not
    /// </summary>
    /// <param name="value">Value to validate</param>
    /// <returns><c>true</c> if given value represents a boolean true-value, otherwise <c>false</c>.</returns>
    private bool IsBooleanTrueValue(string value)
    {
      return new string[] { "y", "yes", "true", "on", "1" }.Contains(value, StringComparer.InvariantCultureIgnoreCase);
    }


    /// <summary>
    /// Gets an indication whether a command value is valid for a custom data-ytpe or not
    /// </summary>
    /// <param name="type">Custom data-type</param>
    /// <param name="name">Property name</param>
    /// <param name="value">Command value</param>
    /// <param name="required">Indicator whether that value has to be required or not</param>
    /// <returns><c>true</c> if custom data-ytpe is valid, otherwise <c>false</c></returns>
    private bool IsValidCustomDataTypeValue(Type type, string name, string value, bool required)
    {
      bool Result = false;

      if (this.CustomDataTypeHandlers.ContainsKey(type))
      {
        CustomDataTypeHandler oHandlerSet = this.CustomDataTypeHandlers[type];

        Result = oHandlerSet.Validator(name, value, required);
      }

      return Result;
    }

    /// <summary>
    /// Gets the value of a custom data-type based on a command value
    /// </summary>
    /// <param name="type">Custom data-type</param>
    /// <param name="name">Property name</param>
    /// <param name="value">Command value of the property</param>
    /// <returns>Value of custom data-type based on a command value</returns>
    private object? GetCustomDataTypeValue(Type type, string name, string value)
    {
      object? Result = null;

      if (this.CustomDataTypeHandlers.ContainsKey(type))
      {
        CustomDataTypeHandler oHandler = this.CustomDataTypeHandlers[type];

        Result = oHandler.Getter(name, value);
      }

      return Result;
    }

    /// <summary>
    /// Defines a custom data-type handler
    /// </summary>
    private class CustomDataTypeHandler
    {
      /// <summary>
      /// Gets or sets the property getter of a custom data-type handler
      /// </summary>
      public PropertyServiceCustomDataTypeGetter Getter
      {
        get;
        set;
      }

      /// <summary>
      /// Gets or sets the property validator of a custom data-type handler
      /// </summary>
      public PropertyServiceCustomDataTypeValidator Validator
      {
        get;
        set;
      }

      /// <summary>
      /// Creates a new instance of the custom data-type handler
      /// </summary>
      /// <param name="getter">Getter for custom data-type property</param>
      /// <param name="validator">validator for custom data-type property</param>
      public CustomDataTypeHandler(PropertyServiceCustomDataTypeGetter getter, PropertyServiceCustomDataTypeValidator validator)
      {
        this.Getter = getter ?? throw new ArgumentNullException(nameof(getter));
        this.Validator = validator ?? throw new ArgumentNullException(nameof(validator));
      }
    }

    private object? _Config;
  }
}
