using HarmonyLib;
using RandomDupes.Randomization;

namespace RandomDupes.Patches
{
    [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    internal static class DbInitializePatch
    {
        private static void Postfix()
        {
            AppearancePools.Rebuild();
        }
    }
}
