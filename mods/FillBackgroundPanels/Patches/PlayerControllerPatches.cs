using System.Linq;
using FillBackgroundPanels.Tools;
using HarmonyLib;
using UnityEngine;

namespace FillBackgroundPanels.Patches
{
    // PlayerController.tools e um array serializado que so contem as ferramentas
    // desenhadas no editor da Klei. Sem esta injecao, ToolMenu ate mostraria o
    // botao (via ToolMenu.basicTools), mas ToolMenu.ChooseTool nunca encontraria
    // um InterfaceTool correspondente em PlayerController.Instance.tools e o
    // clique no botao nao faria nada.
    [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
    public static class PlayerController_OnPrefabInit_Patch
    {
        public static void Prefix(PlayerController __instance)
        {
            if (__instance == null || __instance.tools == null)
                return;

            if (__instance.tools.Any(tool => tool != null && tool.gameObject.name == FillBackgroundTool.ToolName))
                return;

            GameObject template = new GameObject(FillBackgroundTool.ToolName);
            template.SetActive(false);
            FillBackgroundTool tool = template.AddComponent<FillBackgroundTool>();

            InterfaceTool[] expanded = new InterfaceTool[__instance.tools.Length + 1];
            __instance.tools.CopyTo(expanded, 0);
            expanded[expanded.Length - 1] = tool;
            __instance.tools = expanded;
        }
    }
}
