using System;
using HarmonyLib;
using RandomDupes.Configuration;
using RandomDupes.Diagnostics;
using RandomDupes.Randomization;
using UnityEngine;

namespace RandomDupes.Patches
{
    [HarmonyPatch(typeof(CharacterContainer), "OnSpawn")]
    internal static class CharacterContainerPatches
    {
        private const string ButtonName = "RandomDupesAppearanceRerollButton";

        private static void Postfix(CharacterContainer __instance, KButton ___reshuffleButton)
        {
            try
            {
                if (__instance == null || ___reshuffleButton == null ||
                    !SettingsResolver.Current().EnablePrinterAppearanceReroll ||
                    ___reshuffleButton.transform.parent.Find(ButtonName) != null)
                    return;

                GameObject buttonObject = UnityEngine.Object.Instantiate(
                    ___reshuffleButton.gameObject,
                    ___reshuffleButton.transform.parent);
                buttonObject.name = ButtonName;
                buttonObject.SetActive(true);

                KButton button = buttonObject.GetComponent<KButton>();
                button.ClearOnClick();
                button.onClick += () => Reroll(__instance);

                LocText label = buttonObject.GetComponentInChildren<LocText>();
                if (label != null)
                    label.SetText("Randomize Appearance");
            }
            catch (Exception exception)
            {
                ErrorLogger.Write("Creating the Printing Pod appearance reroll button", exception);
            }
        }

        private static void Reroll(CharacterContainer container)
        {
            try
            {
                MinionStartingStats stats = container?.Stats;
                if (stats == null)
                    return;
                DupeAppearanceRandomizer.RerollPhysicalAppearance(stats);
                container.SetMinion(stats);
            }
            catch (Exception exception)
            {
                ErrorLogger.Write("Rerolling physical appearance in the Printing Pod", exception);
            }
        }
    }
}
