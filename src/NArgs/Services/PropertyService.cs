using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NArgs.Attributes;
using NArgs.Models;
using NExtents;

namespace NArgs.Services;

/// <summary>
/// Defines the default property service for parsing arguments.
/// </summary>
public sealed class PropertyService : IPropertyService
{
    private object _config;

    /// <summary>
    /// Gets the parse options.
    /// </summary>
    public ParseOptions Options { get; }

    /// <summary>
    /// Gets the configuration used for argument parsing.
    /// </summary>
    private object Config
    {
        get
        {
            if (_config is null)
            {
                throw new InvalidOperationException(Resources.PropertyServiceNotInitializedErrorMessage);
            }

            return _config;
        }
    }

    /// <summary>
    /// Gets the dictionary of registered custom data-type handlers.
    /// </summary>
    private Dictionary<Type, CustomDataTypeHandler> CustomDataTypeHandlers { get; }

    /// <summary>
    /// Initializes a new instance of the default property service.
    /// </summary>
    internal PropertyService()
    {
        Options = new ParseOptions();
        CustomDataTypeHandlers = new Dictionary<Type, CustomDataTypeHandler>();
    }

    /// <summary>
    /// Initializes a new instance of the default property service with custom parse options.
    /// </summary>
    /// <param name="options">Parse options of this property service.</param>
    internal PropertyService(ParseOptions options) : this()
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Init(object config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <inheritdoc />
    public IEnumerable<PropertyInfo> GetProperties()
    {
        return GetProperties(Config);
    }

    /// <inheritdoc />
    public IEnumerable<PropertyInfo> GetCommandProperties(string commandName)
    {
        if (string.IsNullOrWhiteSpace(commandName))
        {
            throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(commandName));
        }

        var commandProperty = GetPropertyByCommandName(commandName) ?? throw new ArgumentException(Resources.CommandDoesNotExistErrorMessage, commandName);

        return GetProperties(commandProperty.GetValue(Config));
    }



    /// <inheritdoc />
    public bool HasCommands()
    {
        return GetProperties().Any(x => x.GetCustomAttributes(typeof(Command), true).Any());
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
                if (option.Name == name ||
                    option.AlternativeName == name ||
                    option.LongName == name)
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
                if (option.Name == name ||
                    option.AlternativeName == name ||
                    option.LongName == name)
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

        foreach (var property in GetProperties())
        {
            if (property.GetCustomAttributes(typeof(Command), true).FirstOrDefault() is Command command)
            {
                if (string.Compare(command.Name, name, StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(command.LongName, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return property;
                }
            }
        }

        throw new ArgumentException(Resources.CommandDoesNotExistErrorMessage, name);
    }

    /// <inheritdoc />
    public object GetCommandByName(string name)
    {
        return GetPropertyByCommandName(name).GetValue(Config);
    }

    /// <inheritdoc />
    public bool IsRequired(PropertyInfo info)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        if (info.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
        {
            return option.Required;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc />
    public string GetOptionName(PropertyInfo info)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        if (info.GetCustomAttributes(typeof(Option), true).FirstOrDefault() is Option option)
        {
            return option.Name ?? option.AlternativeName ?? option.LongName ?? string.Empty;
        }
        else
        {
            throw new ArgumentException(Resources.PropertyDoesNotHaveAnOptionAttributeErrorMessage, nameof(info));
        }
    }

    /// <inheritdoc />
    public void SetPropertyValue(PropertyInfo property, string value)
    {
        SetPropertyValue(property, Config, value);
    }

    /// <inheritdoc />
    public void SetCommandPropertyValue(PropertyInfo property,
        string commandName,
        string value)
    {
        if (string.IsNullOrWhiteSpace(commandName))
        {
            throw new ArgumentException(Resources.MissingRequiredParameterValueErrorMessage, nameof(commandName));
        }

        var commandProperty = GetPropertyByCommandName(commandName) ?? throw new ArgumentException(Resources.CommandDoesNotExistErrorMessage, nameof(commandName));

        SetPropertyValue(property, commandProperty.GetValue(Config), value);
    }

    /// <inheritdoc />
    public void AddCustomDataTypeHandler(Type type,
        PropertyServiceCustomDataTypeGetter getter,
        PropertyServiceCustomDataTypeValidator validator)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (getter is null)
        {
            throw new ArgumentNullException(nameof(getter));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        CustomDataTypeHandlers.Add(type.GetType(), new CustomDataTypeHandler(getter, validator));
    }

    /// <summary>
    /// Checks whether a value is valid for a given property.
    /// </summary>
    /// <param name="info">Property information.</param>
    /// <param name="value">Value to validate.</param>
    /// <returns><see langword="true" /> if value is valid, otherwise <see langword="false" />.</returns>
    public bool IsValidValue(PropertyInfo info, string value)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        bool result;
        var isValueRequired = IsRequired(info);

        // if args has no value, property has to be of type "bool" - otherwise check type against given value
        if (string.IsNullOrWhiteSpace(value))
        {
            return (info.PropertyType.FullName == PropertyTypeFullName.Boolean);
        }
        else
        {
            switch (info.PropertyType.FullName)
            {
                case PropertyTypeFullName.Char:
                    result = char.TryParse(value, out var _);
                    break;

                case PropertyTypeFullName.String:
                    result = true;
                    break;

                case PropertyTypeFullName.Boolean:
                    result = IsBooleanTrueValue(value)
                             || IsBooleanFalseValue(value);
                    break;

                case PropertyTypeFullName.DateTime:
                    result = DateTime.TryParse(value, out var _);
                    break;

                case PropertyTypeFullName.DirectoryInfo:
                    result = FileSystemInfo.IsValidDirectoryName(value);

                    if (result == true
                        && isValueRequired)
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
                    result = IsValidCustomDataTypeValue(info.PropertyType.GetType(), info.Name, value, IsRequired(info));
                    break;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public bool IsValidOptionValue(string name, string value)
    {
        var property = GetPropertyByOptionName(name);

        if (property is null)
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
    /// Gets all properties of given object.
    /// </summary>
    /// <param name="target">Target object to get properties for.</param>
    /// <returns>List of all properties of given object.</returns>
    private IEnumerable<PropertyInfo> GetProperties(object target)
    {
        return target
               .GetType()
               .GetProperties();
    }

    /// <summary>
    /// Gets an indicator whether a given value represents a boolean false-value.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns><see langword="true" /> if given value represents a boolean false-value, otherwise <see langword="false" />.</returns>
    private bool IsBooleanFalseValue(string value)
    {
        return new string[] { "n", "no", "false", "off", "0" }.Contains(value, StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Gets an indicator whether a given value represents a boolean true-value.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns><see langword="true" /> if given value represents a boolean true-value, otherwise <see langword="false" />.</returns>
    private bool IsBooleanTrueValue(string value)
    {
        return new string[] { "y", "yes", "true", "on", "1" }.Contains(value, StringComparer.InvariantCultureIgnoreCase);
    }


    /// <summary>
    /// Gets an indication whether a command value is valid for a custom data-type.
    /// </summary>
    /// <param name="type">Custom data-type.</param>
    /// <param name="name">Property name.</param>
    /// <param name="value">Command value.</param>
    /// <param name="required">Indicator whether that value is required.</param>
    /// <returns><see langword="true" /> if custom data-type is valid, otherwise <see langword="false" />.</returns>
    private bool IsValidCustomDataTypeValue(Type type,
        string name,
        string value,
        bool required)
    {
        var result = false;

        if (CustomDataTypeHandlers.ContainsKey(type))
        {
            var handler = CustomDataTypeHandlers[type];

            result = handler.Validator(name, value, required);
        }

        return result;
    }

    /// <summary>
    /// Sets a property value for given object.
    /// </summary>
    /// <param name="property">Property information.</param>
    /// <param name="target">Target object.</param>
    /// <param name="value">Value to set.</param>
    /// <exception cref="ArgumentNullException">Property has not been provided.</exception>
    /// <exception cref="ArgumentException">Property with no value is allowed for type <see cref="bool" /> only.</exception>
    private void SetPropertyValue(PropertyInfo property,
        object target,
        string value)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        // if args has no value, property has to be of type "bool" - otherwise check type against given value
        if (string.IsNullOrWhiteSpace(value))
        {
            if (property.PropertyType.FullName == PropertyTypeFullName.Boolean)
            {
                property.SetValue(target, true);
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
                    property.SetValue(target, value);
                    break;

                case PropertyTypeFullName.Boolean:
                    if (IsBooleanTrueValue(value))
                    {
                        property.SetValue(target, true);
                    }
                    else if (IsBooleanFalseValue(value))
                    {
                        property.SetValue(target, false);
                    }
                    break;

                case PropertyTypeFullName.Char:
                    property.SetValue(target, char.Parse(value));
                    break;

                case PropertyTypeFullName.DateTime:
                    property.SetValue(target, DateTime.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.Double:
                    property.SetValue(target, double.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.Int16:
                    property.SetValue(target, short.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.Int32:
                    property.SetValue(target, int.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.Int64:
                    property.SetValue(target, long.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.FileInfo:
                    property.SetValue(target, new FileInfo(value));
                    break;

                case PropertyTypeFullName.DirectoryInfo:
                    property.SetValue(target, new DirectoryInfo(value));
                    break;

                case PropertyTypeFullName.Single:
                    property.SetValue(target, float.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.UInt16:
                    property.SetValue(target, ushort.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.UInt32:
                    property.SetValue(target, uint.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.UInt64:
                    property.SetValue(target, ulong.Parse(value, Options.Culture));
                    break;

                case PropertyTypeFullName.Uri:
                    property.SetValue(target, new Uri(value));
                    break;

                default:
                    property.SetValue(target, GetCustomDataTypeValue(property.PropertyType.GetType(), property.Name, value));
                    break;
            }
        }
    }

    /// <summary>
    /// Gets the value of a custom data-type based on a command value.
    /// </summary>
    /// <param name="type">Custom data-type.</param>
    /// <param name="name">Property name.</param>
    /// <param name="value">Command value of the property.</param>
    /// <returns>Value of custom data-type based on a command value.</returns>
    private object GetCustomDataTypeValue(Type type,
        string name,
        string value)
    {
        object result = null;

        if (CustomDataTypeHandlers.ContainsKey(type))
        {
            var handler = CustomDataTypeHandlers[type];

            result = handler.Getter(name, value);
        }

        return result;
    }

    /// <summary>
    /// Defines a custom data-type handler.
    /// </summary>
    private class CustomDataTypeHandler
    {
        /// <summary>
        /// Gets or sets the property getter of a custom data-type handler.
        /// </summary>
        public PropertyServiceCustomDataTypeGetter Getter { get; set; }

        /// <summary>
        /// Gets or sets the property validator of a custom data-type handler.
        /// </summary>
        public PropertyServiceCustomDataTypeValidator Validator { get; set; }

        /// <summary>
        /// Creates a new instance of the custom data-type handler.
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
