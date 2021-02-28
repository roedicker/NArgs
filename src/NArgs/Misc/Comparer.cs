using System;

namespace NArgs
{
  /// <summary>
  /// Comparer for analyzing equality of objects.
  /// </summary>
  internal static class Comparer
  {
    public static bool IsEqual(string? s1, string? s2)
    {
      return ReferenceEquals(s1, s2) || (s1 != null && s1.Equals(s2, StringComparison.Ordinal));
    }
  }
}
