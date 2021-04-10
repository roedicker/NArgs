using System;
using System.Linq;
using System.Reflection;
using NArgs.Attributes;

namespace NArgs.Extensions
{
  /// <summary>
  /// Extensions of a <see cref="PropertyInfo" /> instance.
  /// </summary>
  internal static class PropertyInfoExtensions
  {
    /// <summary>
    /// Gets an indicator whether a property has a <see cref="CommandAttribute" /> attached to it.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <returns><see langword="true" /> if any <see cref="CommandAttribute" /> has been attached to given property,
    /// otherwise <see langword="false" />.</returns>
    public static bool IsCommand(this PropertyInfo prop)
    {
      return prop.GetCustomAttributes(typeof(CommandAttribute), true).Any();
    }

    /// <summary>
    /// Gets the attached <see cref="CommandAttribute" /> of a property.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <returns>Attached <see cref="CommandAttribute"/> or <see langword="null" /> if not available.</returns>
    public static CommandAttribute? GetCommand(this PropertyInfo prop)
    {
      return prop.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault() as CommandAttribute;
    }

    /// <summary>
    /// Gets an indicator whether a property has an <see cref="OptionAttribute" /> attached to it.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <returns><see langword="true" /> if any <see cref="OptionAttribute" /> has been attached to given property,
    /// otherwise <see langword="false" />.</returns>
    public static bool IsOption(this PropertyInfo prop)
    {
      return prop.GetCustomAttributes(typeof(OptionAttribute), true).Any();
    }

    /// <summary>
    /// Gets the attached <see cref="OptionAttribute" /> of a property.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <returns>Attached <see cref="OptionAttribute"/> or <see langword="null" /> if not available.</returns>
    public static OptionAttribute? GetOption(this PropertyInfo prop)
    {
      return prop.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() as OptionAttribute;
    }

    /// <summary>
    /// Gets an indicator whether a property has a <see cref="ParameterAttribute" /> attached to it.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <returns><see langword="true" /> if any <see cref="ParameterAttribute" /> has been attached to given property,
    /// otherwise <see langword="false" />.</returns>
    public static bool IsParameter(this PropertyInfo prop)
    {
      return prop.GetCustomAttributes(typeof(ParameterAttribute), true).Any();
    }

    /// <summary>
    /// Gets the attached <see cref="ParameterAttribute" /> of a property.
    /// </summary>
    /// <param name="prop">Property information.</param>
    /// <returns>Attached <see cref="ParameterAttribute"/> or <see langword="null" /> if not available.</returns>
    public static ParameterAttribute? GetParameter(this PropertyInfo prop)
    {
      return prop.GetCustomAttributes(typeof(ParameterAttribute), true).FirstOrDefault() as ParameterAttribute;
    }
  }
}
