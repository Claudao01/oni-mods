using System;
using System.IO;
using PeterHan.PLib.Options;
using RandomDupes.Diagnostics;
using UnityEngine;

namespace RandomDupes.Configuration
{
    public static class RandomDupesSaveOptionsProvider
    {
        private static RandomDupesOptions globalBackup;
        private static bool editorOpen;
        private static System.DateTime editorFileTimestampUtc;

        public static string GetPerSaveOptionsTitle() => "[CLD01] Random Dupes";

        public static void ShowPerSaveOptions()
        {
            if (editorOpen || RandomDupesSaveSettings.Instance == null)
                return;

            try
            {
                editorOpen = true;
                globalBackup = RandomDupesOptions.Load().Clone();
                POptions.WriteSettings(RandomDupesSaveSettings.ReadCurrentOrGlobal().Clone());
                string path = POptions.GetConfigFilePath(typeof(RandomDupesOptions));
                editorFileTimestampUtc = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : System.DateTime.MinValue;
                POptions.ShowDialog(typeof(RandomDupesOptions), OnEditorClosed);
            }
            catch (Exception exception)
            {
                RestoreGlobals();
                ErrorLogger.Write("Opening save-specific options", exception);
            }
        }

        public static void ResetPerSaveOptions()
        {
            if (RandomDupesSaveSettings.ClearOverride())
                Debug.Log("[RandomDupes] Save override removed; this colony now uses global defaults.");
        }

        private static void OnEditorClosed(object result)
        {
            try
            {
                string path = POptions.GetConfigFilePath(typeof(RandomDupesOptions));
                bool confirmed = File.Exists(path) &&
                    System.DateTime.Compare(File.GetLastWriteTimeUtc(path), editorFileTimestampUtc) > 0;
                if (confirmed && result is RandomDupesOptions options)
                    RandomDupesSaveSettings.ApplyOverride(options.Clone());
            }
            catch (Exception exception)
            {
                ErrorLogger.Write("Saving save-specific options", exception);
            }
            finally
            {
                RestoreGlobals();
            }
        }

        private static void RestoreGlobals()
        {
            try
            {
                if (globalBackup != null)
                    POptions.WriteSettings(globalBackup);
            }
            catch (Exception exception)
            {
                ErrorLogger.Write("Restoring global options after save editor", exception);
            }
            finally
            {
                globalBackup = null;
                editorFileTimestampUtc = System.DateTime.MinValue;
                editorOpen = false;
            }
        }
    }
}
