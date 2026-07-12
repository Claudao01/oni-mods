using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace CLD01_MeteorDigOrders.Patches
{
    [HarmonyPatch(typeof(UnstableGroundManager), "RemoveFromPending")]
    internal static class UnstableGroundManager_RemoveFromPending_Patch
    {
        private static void Postfix(UnstableGroundManager __instance, int cell)
        {
            MeteorDigOrdersController controller = MeteorDigOrdersController.Instance;
            if (controller == null || __instance == null)
                return;

            int worldId = __instance.GetMyWorldId();
            if (controller.IsWatchingUnstableWorld(worldId))
                controller.ProcessSettledUnstableCell(worldId, cell);
        }
    }

    [HarmonyPatch(typeof(UnstableGroundManager), "Update")]
    internal static class UnstableGroundManager_Update_Patch
    {
        private static void Postfix(
            UnstableGroundManager __instance,
            List<GameObject> ___fallingObjects,
            List<int> ___pendingCells)
        {
            MeteorDigOrdersController controller = MeteorDigOrdersController.Instance;
            if (controller == null || __instance == null)
                return;

            int worldId = __instance.GetMyWorldId();
            if (!controller.IsWatchingUnstableWorld(worldId))
                return;

            if ((___fallingObjects == null || ___fallingObjects.Count == 0) &&
                (___pendingCells == null || ___pendingCells.Count == 0) &&
                Components.Meteors.GetItems(worldId).Count == 0)
            {
                controller.StopWatchingUnstableWorld(worldId);
            }
        }
    }
}
