using System.IO;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using UnityEngine;

namespace FillWithBackwalls
{
    public sealed class FillWithBackwallsMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            PUtil.InitLibrary();
            LocString.CreateLocStringKeys(typeof(STRINGS), "FillWithBackwalls.");
            new POptions().RegisterOptions(this, typeof(FillWithBackwallsOptions));
            new PLocalization().Register();

            EnsureDefaultConfig();
            Debug.Log($"[FillWithBackwalls] Mod carregado. MaxCavitySize = {FillWithBackwallsOptions.Load().MaxCavitySize}.");
        }

        private static void EnsureDefaultConfig()
        {
            string configPath = POptions.GetConfigFilePath(typeof(FillWithBackwallsOptions));
            if (!File.Exists(configPath))
                POptions.WriteSettings(new FillWithBackwallsOptions());
        }
    }
}
