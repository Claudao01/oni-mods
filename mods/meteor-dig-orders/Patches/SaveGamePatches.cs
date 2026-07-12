using HarmonyLib;

namespace CLD01_MeteorDigOrders.Patches
{
    [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
    internal static class SaveGame_OnPrefabInit_Patch
    {
        private static void Postfix(SaveGame __instance)
        {
            if (__instance != null)
                __instance.gameObject.AddOrGet<MeteorDigOrdersController>();
        }
    }
}
