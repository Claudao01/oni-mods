using HarmonyLib;
using RandomDupes.Diagnostics;
using RandomDupes.Randomization;

namespace RandomDupes.Patches
{
    [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    internal static class DbInitializePatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Postfix(Db __instance)
        {
            try
            {
                if (__instance?.Personalities == null || __instance.AccessorySlots == null)
                {
                    ErrorLogger.Write(
                        "Building appearance pools after database initialization",
                        "Db.Initialize completed without Personalities or AccessorySlots.");
                    return;
                }

                AppearancePools.Rebuild(__instance);
                PersonalityCatalogExporter.Export(__instance);
            }
            catch (System.Exception exception)
            {
                ErrorLogger.Write("Building appearance pools after database initialization", exception);
            }
        }
    }
}
