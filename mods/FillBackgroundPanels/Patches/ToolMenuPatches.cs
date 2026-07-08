using FillBackgroundPanels.Tools;
using HarmonyLib;

namespace FillBackgroundPanels.Patches
{
    [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
    public static class ToolMenu_CreateBasicTools_Patch
    {
        public static void Postfix(ToolMenu __instance)
        {
            if (__instance == null || __instance.basicTools == null)
                return;

            __instance.basicTools.Add(ToolMenu.CreateToolCollection(
                "Preencher Parede de Fundo",
                "icon_action_dig",
                Action.NumActions,
                FillBackgroundTool.ToolName,
                "Preenche uma cavidade fechada com o predio, material e skin selecionados, e atualiza instantaneamente a skin das paredes de fundo compativeis ja construidas.",
                false));
        }
    }
}
