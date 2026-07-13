using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using UnityEngine;

namespace PerSaveModSettings
{
    public sealed class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            Debug.Log("[PerSaveModSettings] Per-Save Mod Settings 0.1.0 loaded.");
        }
    }
}
