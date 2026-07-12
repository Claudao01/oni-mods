using HarmonyLib;

namespace CLD01_MeteorDigOrders.Patches
{
    [HarmonyPatch(typeof(Comet), "OnCleanUp")]
    internal static class Comet_OnCleanUp_Patch
    {
        private static void Postfix(Comet __instance)
        {
            MeteorDigOrdersController controller = MeteorDigOrdersController.Instance;
            if (controller == null || __instance == null)
                return;

            controller.ProcessLastMeteorCleanup(__instance.GetMyWorldId());
        }
    }
}
