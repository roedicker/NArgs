using System;
using System.Collections.Generic;
using System.Reflection;

namespace NArgs;

/// <summary>
/// Defines the signature of the property-getter for custom data-types based on a command value.
/// </summary>
/// <param name="name">Name of the property.</param>
/// <param name="value">Command value.</param>
/// <returns>Property value based on command value.</returns>
public delegate object PropertyServiceCustomDataTypeGetter(string name, string value);

/// <summary>
/// Defines the signature for validating a custom data-type property.
/// </summary>
/// <param name="name">Name of the property.</param>
/// <param name="value">Command value.</param>
/// <param name="required">Indicator whether this property is required.</param>
/// <returns><see langword="true" /> if named property is valid based on the command value, otherwise <see langword="false"/></returns>
public delegate bool PropertyServiceCustomDataTypeValidator(string name,
    string value,
    bool required);

/// <summary>
/// Defines a property service.
/// </summary>
public interface IPropertyService
{
    /// <summary>
    /// Initializes a property service by a given configuration.
    /// </summary>
    /// <param name="config">Configuration used for parsing command arguments.</param>
    void Init(object config);

    /// <summary>
    /// Gets the property-type based on an option name.
    /// </summary>
    /// <param name="name">Name of the option.</param>
    /// <returns>Data-type of the option.</returns>
    /// <exception cref="ArgumentException">Option by name does not exist.</exception>
    Type GetPropertyTypeByOptionName(string name);

    /// <summary>
    /// Gets the name of a property-type based on an option name.
    /// </summary>
    /// <param name="name">Name of the option.</param>
    /// <returns>Name of a data-type of the option.</returns>
    /// <exception cref="ArgumentException">Option by name does not exist.</exception>
    string GetPropertyTypeNameByOptionName(string name);

    /// <summary>
    /// Gets the property information of an option by its name.
    /// </summary>
    /// <param name="name">Name of the option.</param>
    /// <returns>Property information of given option.</returns>
    /// <exception cref="ArgumentException">Option by name does not exist.</exception>
    PropertyInfo GetPropertyByOptionName(string name);

    /// <summary>
    /// Gets the property information of a command by its name.
    /// </summary>
    /// <param name="name">Command name.</param>
    /// <returns>Property information of given command by its name.</returns>
    /// <exception cref="ArgumentException">Command by name does not exist.</exception>
    PropertyInfo GetPropertyByCommandName(string name);

    /// <summary>
    /// Gets an indicator whether a property is required.
    /// </summary>
    /// <param name="info">Property information.</param>
    /// <returns><see langword="true" /> if property is required, otherwise <see langword="false"/>.</returns>
    bool IsRequired(PropertyInfo info);

    /// <summary>
    /// Gets the name of an option-property.
    /// </summary>
    /// <param name="info">Property information of an option.</param>
    /// <returns>Name of the option.</returns>
    string GetOptionName(PropertyInfo info);

    /// <summary>
    /// Gets all property information based on the configuration.
    /// </summary>
    /// <returns>All property information based on the configuration.</returns>
    IEnumerable<PropertyInfo> GetProperties();

    /// <summary>
    /// Gets all property information based on a given command name.
    /// </summary>
    /// <param name="commandName">Command name.</param>
    /// <returns>All property information based on the given command name.</returns>
    IEnumerable<PropertyInfo> GetCommandProperties(string commandName);

    /// <summary>
    /// Gets the command configuration object by its name.
    /// </summary>
    /// <param name="name">Command name.</param>
    /// <returns>Object in configuration matching command name.</returns>
    /// <exception cref="ArgumentException">Command by name does not exist.</exception>
    object GetCommandByName(string name);

    /// <summary>
    /// Gets an indicator whether the current configuration has commands.
    /// </summary>
    /// <returns><see langword="true" /> if current configuration has commands, otherwise <see langword="false" />.</returns>
    bool HasCommands();

    /// <summary>
    /// Sets a property value.
    /// </summary>
    /// <param name="property">Property information.</param>
    /// <param name="value">Value to be set.</param>
    void SetPropertyValue(PropertyInfo property, string value);

    /// <summary>
    /// Sets a command property value.
    /// </summary>
    /// <param name="property">Property information.</param>
    /// <param name="commandName">Command name.</param>
    /// <param name="value">Value to be set.</param>
    void SetCommandPropertyValue(PropertyInfo property,
        string commandName,
        string value);

    /// <summary>
    /// Gets an indicator whether a property value is valid or not.
    /// </summary>
    /// <param name="property">Property information.</param>
    /// <param name="value">Value to be validated.</param>
    /// <returns><see langword="true" /> if property value is valid, otherwise <see langword="false"/>.</returns>
    bool IsValidValue(PropertyInfo property, string value);

    /// <summary>
    /// Gets an indicator whether an option value is valid or not
    /// </summary>
    /// <param name="name">Name of the option</param>
    /// <param name="value">Value to be validated</param>
    /// <returns><see langword="true" /> if option value is valid, otherwise <see langword="false" />.</returns>
    bool IsValidOptionValue(string name, string value);

    /// <summary>
    /// Adds a handler for a custom data-type.
    /// </summary>
    /// <param name="type">Data-type that should be handled.</param>
    /// <param name="setter">Setter of the custom data-type.</param>
    /// <param name="validator">Validator of the custom data-type.</param>
    void AddCustomDataTypeHandler(Type type,
        PropertyServiceCustomDataTypeGetter setter,
        PropertyServiceCustomDataTypeValidator validator);
}
