using HarmonyLib;
using KMod;
using UnityEngine;

namespace FillBackgroundPanels
{
    public class FillBackgroundPanelsMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            FillBackgroundConfig config = FillBackgroundConfig.Instance;
            Debug.Log($"[FillBackgroundPanels] Mod carregado. MaxCavitySize = {config.MaxCavitySize}.");
        }
    }
}
