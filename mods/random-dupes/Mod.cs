using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using RandomDupes.Configuration;
using RandomDupes.Diagnostics;
using UnityEngine;

namespace RandomDupes
{
    public sealed class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(RandomDupesOptions));

            string configPath = POptions.GetConfigFilePath(typeof(RandomDupesOptions));
            ErrorLogger.Initialize(mod?.ContentPath ?? path, Path.GetDirectoryName(configPath));
            if (!File.Exists(configPath))
                POptions.WriteSettings(new RandomDupesOptions());

            Debug.Log("[RandomDupes] Random Dupes 1.0.0 loaded. Clothing randomization is disabled by design.");
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            try
            {
                DetectApplyAccessoriesConflicts(harmony.Id);
            }
            catch (System.Exception exception)
            {
                ErrorLogger.Write("Inspecting mod compatibility", exception);
            }
        }

        private static void DetectApplyAccessoriesConflicts(string ownHarmonyId)
        {
            MethodInfo method = AccessTools.Method(typeof(MinionStartingStats), nameof(MinionStartingStats.ApplyAccessories));
            HarmonyLib.Patches patches = method == null ? null : Harmony.GetPatchInfo(method);
            if (patches == null)
                return;

            foreach (string owner in patches.Owners)
            {
                if (owner != ownHarmonyId)
                    Debug.LogWarning($"[RandomDupes] Compatibility notice: '{owner}' also patches MinionStartingStats.ApplyAccessories.");
            }
        }
    }
}
