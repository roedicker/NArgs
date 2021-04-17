using System;
using System.Collections.Generic;
using System.Reflection;

using NArgs.Attributes;

namespace NArgs
{
  /// <summary>
  /// Defines the signature of the property-getter for custom data-types based on a command value.
  /// </summary>
  /// <param name="name">Name of the property.</param>
  /// <param name="value">Command value.</param>
  /// <returns>Property value based on command value.</returns>
  public delegate object PropertyServiceCustomDataTypeGetter(string name, string? value);

  /// <summary>
  /// Defines the signature for validating a custom data-type property.
  /// </summary>
  /// <param name="name">Name of the property.</param>
  /// <param name="value">Command value.</param>
  /// <param name="required">Indicator whether this property is required or not.</param>
  /// <returns>Indicator whether named property is valid based on the command value or not, based on its value.</returns>
  public delegate bool PropertyServiceCustomDataTypeValidator(string name, string? value, bool required);

  /// <summary>
  /// Defines a property service.
  /// </summary>
  public interface IPropertyService
  {
    /// <summary>
    /// Gets the property-type based on an option name.
    /// </summary>
    /// <param name="name">Name of the option</param>
    /// <returns>Data-type of the option.</returns>
    Type GetPropertyTypeByOptionName(string name);

    /// <summary>
    /// Gets the name of a property-type based on an option name.
    /// </summary>
    /// <param name="name">Name of the option.</param>
    /// <returns>Name of a data-type of the option.</returns>
    string GetPropertyTypeNameByOptionName(string name);

    /// <summary>
    /// Gets the property information of an option by its name.
    /// </summary>
    /// <param name="name">Name of the option.</param>
    /// <returns>Property information of given option.</returns>
    PropertyInfo GetPropertyByOptionName(string name);


    /// <summary>
    /// Gets the property information of a command by its name.
    /// </summary>
    /// <param name="name">Name of the command.</param>
    /// <returns>Property information of given command.</returns>
    PropertyInfo GetPropertyByCommandName(string name);

    /// <summary>
    /// Gets an indicator whether a property is required or not.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <returns>Indicator whether a property is required or not.</returns>
    bool IsRequired(PropertyInfo prop);

    /// <summary>
    /// Gets the name of an option-property.
    /// </summary>
    /// <param name="prop">Property information of an option.</param>
    /// <returns>Name of the option.</returns>
    string GetOptionName(PropertyInfo prop);

    /// <summary>
    /// Gets the option of given property.
    /// </summary>
    /// <param name="prop">Property information of an option.</param>
    /// <returns>Option attribute.</returns>
    OptionAttribute GetOption(PropertyInfo prop);

    /// <summary>
    /// Gets the parameter of given property.
    /// </summary>
    /// <param name="prop">Property information of a parameter.</param>
    /// <returns>Parameter attribute.</returns>
    ParameterAttribute GetParameter(PropertyInfo prop);

    /// <summary>
    /// Gets the command of given property.
    /// </summary>
    /// <param name="prop">Property information of a command.</param>
    /// <returns>Command attribute.</returns>
    CommandAttribute GetCommand(PropertyInfo prop);

    /// <summary>
    /// Gets all property information based on the configuration.
    /// </summary>
    /// <returns>All property information based on the configuration.</returns>
    IEnumerable<PropertyInfo> GetProperties();

    /// <summary>
    /// Gets all property information based on the global configuration.
    /// </summary>
    /// <returns>All property information based on the global configuration.</returns>
    IEnumerable<PropertyInfo> GetGlobalProperties();

    /// <summary>
    /// Gets an indicator whether one of the properties of the configuration is a command.
    /// </summary>
    /// <returns><see langword="true" /> if one of the properties of the configuration is a command, otherwise <see langword="false" />.</returns>
    bool HasCommands();

    /// <summary>
    /// Gets an indicator whether one of the properties of the configuration is an option.
    /// </summary>
    /// <returns><see langword="true" /> if one of the properties of the configuration is an option, otherwise <see langword="false" />.</returns>
    bool HasOptions();

    /// <summary>
    /// Gets an indicator whether one of the properties of the configuration is a parameter.
    /// </summary>
    /// <returns><see langword="true" /> if one of the properties of the configuration is a parameter, otherwise <see langword="false" />.</returns>
    bool HasParameters();

    /// <summary>
    /// Gets all required options names which have not been assigned a value, yet.
    /// </summary>
    /// <returns>All required option names which have not been assigned a value.</returns>
    IEnumerable<string> GetUnassignedRequiredOptionNames();

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <param name="prop">Property information.</param>
    object GetPropertyValue(PropertyInfo prop);

    /// <summary>
    /// Gets a global property value.
    /// </summary>
    /// <param name="prop">Property information.</param>
    object GetGlobalPropertyValue(PropertyInfo prop);

    /// <summary>
    /// Sets the current command configuration.
    /// </summary>
    /// <param name="command">Current command.</param>
    void SetCurrentCommand(object command);

    /// <summary>
    /// Resets the current command configuration.
    /// </summary>
    void ResetCurrentCommand();

    /// <summary>
    /// Sets a property value.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <param name="value">Value to be set.</param>
    void SetPropertyValue(PropertyInfo prop, string? value);

    /// <summary>
    /// Gets an indicator whether a property value is valid or not.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <param name="value">Value to be validated.</param>
    /// <returns>Indicator whether the property value is valid or not.</returns>
    bool IsValidValue(PropertyInfo prop, string? value);

    /// <summary>
    /// Gets an indicator whether an option value is valid or not.
    /// </summary>
    /// <param name="name">Name of the option.</param>
    /// <param name="value">Value to be validated.</param>
    /// <returns>Indicator whether the option value is valid or not.</returns>
    bool IsValidOptionValue(string name, string? value);

    /// <summary>
    /// Adds a handler for a custom data-type.
    /// </summary>
    /// <param name="type">Data-type that should be handled.</param>
    /// <param name="setter">Setter of the custom data-type.</param>
    /// <param name="validator">Validator of the custom data-type.</param>
    void AddCustomDataTypeHandler(Type type, PropertyServiceCustomDataTypeGetter setter, PropertyServiceCustomDataTypeValidator validator);
  }
}
