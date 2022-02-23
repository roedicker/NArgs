using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: InternalsVisibleToAttribute("NArgstest")]

namespace NArgs.Extensions
{
  internal static class SecureStringExtensions
  {
    public static SecureString AppendString(this SecureString s, string value)
    {
      if (s == null)
      {
        throw new ArgumentNullException(nameof(s));
      }

      if (value != null)
      {
        foreach (var c in value)
        {
          s.AppendChar(c);
        }
      }

      return s;
    }

    public static SecureString SetString(this SecureString s, string value)
    {
      if (s == null)
      {
        throw new ArgumentNullException(nameof(s));
      }

      s.Clear();

      return AppendString(s, value);
    }

    public static bool IsEmpty(this SecureString s)
    {
      if (s == null)
      {
        throw new ArgumentNullException(nameof(s));
      }

      return (s.Length == 0);
    }

    public static string GetString(this SecureString s)
    {
      if (s == null)
      {
        throw new ArgumentNullException(nameof(s));
      }

      var result = string.Empty; ;
      var value = IntPtr.Zero;

      if ((s is not null) && (s.Length > 0))
      {
        try
        {
          value = Marshal.SecureStringToBSTR(s);
          result = Marshal.PtrToStringBSTR(value);
        }
        finally
        {
          if (value != IntPtr.Zero)
          {
            Marshal.ZeroFreeBSTR(value);
          }
        }
      }

      return result;
    }
  }
}
