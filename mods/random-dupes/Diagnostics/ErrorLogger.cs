using System;
using System.IO;
using UnityEngine;

namespace RandomDupes.Diagnostics
{
    internal static class ErrorLogger
    {
        private const string FileName = "random-dupes-errors.log";
        private static string primaryPath;
        private static string fallbackPath;

        internal static void Initialize(string installedDirectory, string configDirectory)
        {
            primaryPath = BuildPath(installedDirectory);
            fallbackPath = BuildPath(configDirectory);
        }

        internal static void Write(string context, Exception exception)
        {
            string entry = $"[{System.DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}] {context}{Environment.NewLine}{exception}{Environment.NewLine}";
            Debug.LogError($"[RandomDupes] {context}: {exception}");

            if (!TryAppend(primaryPath, entry))
                TryAppend(fallbackPath, entry);
        }

        internal static void Write(string context, string details)
        {
            Write(context, new InvalidOperationException(details));
        }

        private static string BuildPath(string directory)
        {
            return string.IsNullOrEmpty(directory) ? null : Path.Combine(directory, FileName);
        }

        private static bool TryAppend(string path, string entry)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.AppendAllText(path, entry);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
