using HarmonyLib;
using Klei.AI;

namespace CLD01_MeteorDigOrders.Patches
{
    [HarmonyPatch(typeof(MeteorShowerEvent.StatesInstance), nameof(MeteorShowerEvent.StatesInstance.StopSM))]
    internal static class MeteorShowerEvent_StatesInstance_StopSM_Patch
    {
        private static void Postfix(MeteorShowerEvent.StatesInstance __instance)
        {
            MeteorDigOrdersController controller = MeteorDigOrdersController.Instance;
            if (controller == null || __instance == null)
                return;

            int worldId = Traverse.Create(__instance).Field("m_worldId").GetValue<int>();
            if (worldId >= 0)
                controller.NotifyShowerEnded(worldId);
        }
    }
}
