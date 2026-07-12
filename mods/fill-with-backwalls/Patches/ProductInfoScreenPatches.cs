using HarmonyLib;
using UnityEngine;

namespace FillWithBackwalls.Patches
{
    [HarmonyPatch(typeof(ProductInfoScreen))]
    public static class ProductInfoScreenPatches
    {
        private const string ToggleObjectName = "FillWithBackwallsFillCavityToggle";
        private static MultiToggle fillToggle;
        private static GameObject fillToggleObject;

        [HarmonyPatch(nameof(ProductInfoScreen.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(ProductInfoScreen __instance)
        {
            CreateToggle(__instance);
            RefreshToggle(__instance);
        }

        [HarmonyPatch(nameof(ProductInfoScreen.ConfigureScreen), typeof(BuildingDef), typeof(string))]
        [HarmonyPostfix]
        public static void ConfigureScreen_Postfix(ProductInfoScreen __instance)
        {
            RefreshToggle(__instance);
        }

        [HarmonyPatch(nameof(ProductInfoScreen.ClearProduct))]
        [HarmonyPostfix]
        public static void ClearProduct_Postfix()
        {
            if (fillToggleObject != null)
                fillToggleObject.SetActive(false);
        }

        private static void CreateToggle(ProductInfoScreen screen)
        {
            if (screen == null || fillToggleObject != null || screen.sandboxInstantBuildToggle == null)
                return;

            GameObject template = screen.sandboxInstantBuildToggle.gameObject;
            GameObject parent = template.transform.parent != null ? template.transform.parent.gameObject : screen.gameObject;
            fillToggleObject = Util.KInstantiateUI(template, parent, true);
            fillToggleObject.name = ToggleObjectName;

            RectTransform templateRect = template.rectTransform();
            RectTransform toggleRect = fillToggleObject.rectTransform();
            toggleRect.anchorMin = templateRect.anchorMin;
            toggleRect.anchorMax = templateRect.anchorMax;
            toggleRect.pivot = templateRect.pivot;
            toggleRect.sizeDelta = templateRect.sizeDelta;
            toggleRect.anchoredPosition = templateRect.anchoredPosition + new Vector2(0f, 34f);

            fillToggle = fillToggleObject.GetComponent<MultiToggle>();
            if (fillToggle != null)
            {
                fillToggle.onClick = ToggleFillMode;
                fillToggle.ChangeState(FillWithBackwallsState.IsFillModeActive ? 1 : 0, true);
            }

            ToolTip tooltip = fillToggleObject.GetComponent<ToolTip>() ?? fillToggleObject.AddComponent<ToolTip>();
            tooltip.SetSimpleTooltip(FillWithBackwallsState.Tooltip);

            foreach (LocText label in fillToggleObject.GetComponentsInChildren<LocText>(true))
                label.SetText(FillWithBackwallsState.Title);

            fillToggleObject.SetActive(false);
        }

        private static void ToggleFillMode()
        {
            FillWithBackwallsState.IsFillModeActive = !FillWithBackwallsState.IsFillModeActive;
            if (fillToggle != null)
                fillToggle.ChangeState(FillWithBackwallsState.IsFillModeActive ? 1 : 0);
        }

        private static void RefreshToggle(ProductInfoScreen screen)
        {
            if (screen == null || fillToggleObject == null)
                return;

            bool shouldShow = screen.currentDef != null && screen.currentDef.ObjectLayer == ObjectLayer.Backwall;
            fillToggleObject.SetActive(shouldShow);

            if (fillToggle != null)
                fillToggle.ChangeState(FillWithBackwallsState.IsFillModeActive ? 1 : 0);
        }
    }
}
