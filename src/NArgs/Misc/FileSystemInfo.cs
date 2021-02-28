using System;
using System.IO;
using System.Linq;


namespace NArgs
{
  /// <summary>
  /// Provides informations regarding the file-system
  /// </summary>
  public static class FileSystemInfo
  {
    /// <summary>
    /// Gets an indicator whether a given name is a valid directory name or not
    /// </summary>
    /// <param name="name">Name to validate</param>
    /// <returns><c>true</c> if the given name is valid, otherwise <c>false</c>.</returns>
    public static bool IsValidDirectoryName(string name)
    {
      bool Result = true;

      if (string.IsNullOrWhiteSpace(name))
      {
        Result = false;
      }

      if (Result == true)
      {
        string[] asNames = name.Split(Path.DirectorySeparatorChar);

        for (int i = 0; i < asNames.Length - 1; i++)
        {
          foreach (char cChar in asNames[i])
          {
            if (Path.GetInvalidPathChars().Contains(cChar))
            {
              Result = false;
              break;
            }
          }

          if (Result == false)
          {
            break;
          }
        }
      }

      return Result;
    }

    /// <summary>
    /// Gets an indicator whether a given name is a valid file name or not
    /// </summary>
    /// <param name="name">Name to validate</param>
    /// <returns><c>true</c> if the given name is valid, otherwise <c>false</c>.</returns>
    public static bool IsValidFileName(string name)
    {
      bool Result = true;

      if (string.IsNullOrWhiteSpace(name))
      {
        Result = false;
      }

      if (Result == true)
      {
        string[] asNames = name.Split(Path.DirectorySeparatorChar);

        for (int i = 0; i < asNames.Length; i++)
        {
          if (i < asNames.Length - 2)
          {
            Result = IsValidDirectoryName(asNames[i]);
          }
          else
          {
            foreach (char cChar in asNames[i])
            {
              if (Path.GetInvalidFileNameChars().Contains(cChar))
              {
                Result = false;
                break;
              }
            }
          }

          if (Result == false)
          {
            break;
          }
        }
      }

      return Result;
    }
  }
}
