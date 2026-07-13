using System.Collections.Generic;
using HarmonyLib;
using PerSaveModSettings.UI;
using UnityEngine.Events;

namespace PerSaveModSettings.Patches
{
    [HarmonyPatch(typeof(OptionsMenuScreen), "OnActivate")]
    internal static class OptionsMenuScreenPatch
    {
        private const string ButtonText = "Mod Settings";

        private static void Postfix(OptionsMenuScreen __instance)
        {
            try
            {
                if (__instance == null || SaveGame.Instance == null || !ModSettingsDialog.HasProviders())
                    return;

                Traverse field = Traverse.Create(__instance).Field("buttons");
                IList<KButtonMenu.ButtonInfo> buttons = field.GetValue<IList<KButtonMenu.ButtonInfo>>();
                if (buttons == null)
                    return;
                foreach (KButtonMenu.ButtonInfo button in buttons)
                {
                    if (button != null && button.text == ButtonText)
                        return;
                }

                List<KButtonMenu.ButtonInfo> updated = new List<KButtonMenu.ButtonInfo>(buttons);
                updated.Insert(System.Math.Min(3, updated.Count), new KButtonMenu.ButtonInfo(
                    ButtonText,
                    Action.NumActions,
                    new UnityAction(ModSettingsDialog.Show)));
                field.SetValue(updated);
                __instance.RefreshButtons();
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"[PerSaveModSettings] Could not add Mod Settings to Options: {exception}");
            }
        }
    }
}
