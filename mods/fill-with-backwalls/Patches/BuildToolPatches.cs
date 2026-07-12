using System.Collections.Generic;
using FillWithBackwalls.Tools;
using HarmonyLib;
using UnityEngine;

namespace FillWithBackwalls.Patches
{
    [HarmonyPatch(typeof(BuildTool))]
    public static class BuildToolPatches
    {
        private static readonly AccessTools.FieldRef<BuildTool, BuildingDef> DefRef = AccessTools.FieldRefAccess<BuildTool, BuildingDef>("def");
        private static readonly AccessTools.FieldRef<BuildTool, IList<Tag>> SelectedElementsRef = AccessTools.FieldRefAccess<BuildTool, IList<Tag>>("selectedElements");
        private static readonly AccessTools.FieldRef<BuildTool, Orientation> OrientationRef = AccessTools.FieldRefAccess<BuildTool, Orientation>("buildingOrientation");
        private static readonly AccessTools.FieldRef<BuildTool, string> FacadeIdRef = AccessTools.FieldRefAccess<BuildTool, string>("facadeID");

        private static bool handledCurrentDrag;

        [HarmonyPatch(nameof(BuildTool.OnLeftClickDown))]
        [HarmonyPrefix]
        public static void OnLeftClickDown_Prefix()
        {
            handledCurrentDrag = false;
        }

        [HarmonyPatch(nameof(BuildTool.OnLeftClickUp))]
        [HarmonyPostfix]
        public static void OnLeftClickUp_Postfix()
        {
            handledCurrentDrag = false;
        }

        [HarmonyPatch("TryBuild")]
        [HarmonyPrefix]
        public static bool TryBuild_Prefix(BuildTool __instance, int cell)
        {
            if (!FillWithBackwallsState.IsFillModeActive)
                return true;

            BuildingDef def = DefRef(__instance);
            if (def == null || def.ObjectLayer != ObjectLayer.Backwall)
                return true;

            if (!handledCurrentDrag)
            {
                handledCurrentDrag = true;
                FillWithBackwallsFillService.TryFillCavityAt(
                    cell,
                    def,
                    __instance.visualizer,
                    OrientationRef(__instance),
                    SelectedElementsRef(__instance),
                    FacadeIdRef(__instance),
                    Grid.CellToPosCBC(cell, Grid.SceneLayer.Building));
            }

            return false;
        }
    }
}
