using HarmonyLib;
using RandomDupes.Configuration;

namespace RandomDupes.Patches
{
    [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
    internal static class SaveGamePatches
    {
        private static void Postfix(SaveGame __instance)
        {
            if (__instance != null)
                __instance.gameObject.AddOrGet<RandomDupesSaveSettings>();
        }
    }
}
