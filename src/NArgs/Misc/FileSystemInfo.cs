using System;
using System.IO;
using System.Linq;

namespace NArgs
{
  /// <summary>
  /// Provides informations regarding the file-system.
  /// </summary>
  public static class FileSystemInfo
  {
    /// <summary>
    /// Gets an indicator whether a given name is a valid directory name or not.
    /// </summary>
    /// <param name="name">Name to validate.</param>
    /// <returns><see langword="true" /> if the given name is valid, otherwise <see langword="false" />.</returns>
    public static bool IsValidDirectoryName(string name)
    {
      var result = true;

      if (string.IsNullOrWhiteSpace(name))
      {
        result = false;
      }

      if (result == true)
      {
        var names = name.Split(Path.DirectorySeparatorChar);

        for (var i = 0; i < names.Length - 1; i++)
        {
          foreach (var chracter in names[i])
          {
            if (Path.GetInvalidPathChars().Contains(chracter))
            {
              result = false;
              break;
            }
          }

          if (result == false)
          {
            break;
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Gets an indicator whether a given name is a valid file name or not.
    /// </summary>
    /// <param name="name">Name to validate.</param>
    /// <returns><see langword="true" /> if the given name is valid, otherwise <see langword="false" />.</returns>
    public static bool IsValidFileName(string name)
    {
      var result = true;

      if (string.IsNullOrWhiteSpace(name))
      {
        result = false;
      }

      if (result == true)
      {
        var names = name.Split(Path.DirectorySeparatorChar);

        for (var i = 0; i < names.Length; i++)
        {
          if (i < names.Length - 2)
          {
            result = IsValidDirectoryName(names[i]);
          }
          else
          {
            foreach (var character in names[i])
            {
              if (Path.GetInvalidFileNameChars().Contains(character))
              {
                result = false;
                break;
              }
            }
          }

          if (result == false)
          {
            break;
          }
        }
      }

      return result;
    }
  }
}
