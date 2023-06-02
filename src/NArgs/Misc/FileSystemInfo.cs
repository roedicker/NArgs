using System.IO;
using System.Linq;

namespace NArgs;

/// <summary>
/// Provides information regarding the file-system.
/// </summary>
public static class FileSystemInfo
{
    /// <summary>
    /// Gets an indicator whether a given name is a valid directory name.
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
            var separators = name.Split(Path.DirectorySeparatorChar);

            for (var i = 0; i < separators.Length - 1; i++)
            {
                foreach (var c in separators[i])
                {
                    if (Path.GetInvalidPathChars().Contains(c))
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
    /// Gets an indicator whether a given name is a valid file name.
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
            var separators = name.Split(Path.DirectorySeparatorChar);

            for (var i = 0; i < separators.Length; i++)
            {
                if (i < separators.Length - 2)
                {
                    result = IsValidDirectoryName(separators[i]);
                }
                else
                {
                    foreach (var c in separators[i])
                    {
                        if (Path.GetInvalidFileNameChars().Contains(c))
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
