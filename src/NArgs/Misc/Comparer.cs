using System;

namespace NArgs
{
  /// <summary>
  /// Comparer for analyzing equality of objects.
  /// </summary>
  internal static class Comparer
  {
    /// <summary>
    /// Gets an indicator whether two strings are equal or not.
    /// </summary>
    /// <param name="s1">First string to compare.</param>
    /// <param name="s2">Second string to compare.</param>
    /// <returns><see langword="true" /> if both strings are equal, otherwise <see langword="false" />.</returns>
    public static bool IsEqual(string? s1, string? s2)
    {
      return ReferenceEquals(s1, s2) || (s1 != null && s1.Equals(s2, StringComparison.Ordinal));
    }
  }
}
