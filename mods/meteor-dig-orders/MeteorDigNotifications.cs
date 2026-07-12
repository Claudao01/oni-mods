using System.Collections.Generic;
using UnityEngine;

namespace CLD01_MeteorDigOrders
{
    internal static class MeteorDigNotifications
    {
        internal static void ShowBombardmentStarted(int worldId)
        {
            BuildingComplete focus = MeteorDigService.GetFirstSolarPanel(worldId);
            if (focus == null)
                return;

            Add(
                focus,
                STRINGS.UI.NOTIFICATIONS.BOMBARDMENT_STARTED.ToString(),
                STRINGS.UI.NOTIFICATIONS.BOMBARDMENT_STARTED_TOOLTIP.ToString(),
                NotificationType.BadMinor);
        }

        internal static void ShowResult(int worldId, DigScanResult result)
        {
            BuildingComplete focus = result.FirstAffected ?? MeteorDigService.GetFirstSolarPanel(worldId);
            if (focus == null)
                return;

            if (result.Created == 0 && result.Raised == 0)
            {
                Add(
                    focus,
                    STRINGS.UI.NOTIFICATIONS.NO_CLEANUP_REQUIRED.ToString(),
                    STRINGS.UI.NOTIFICATIONS.NO_CLEANUP_REQUIRED_TOOLTIP.ToString(),
                    NotificationType.Good);
                return;
            }

            string tooltip = string.Format(
                STRINGS.UI.NOTIFICATIONS.CLEANUP_RESULT_TOOLTIP.ToString(),
                result.Created,
                result.Raised);
            Add(
                focus,
                STRINGS.UI.NOTIFICATIONS.CLEANUP_RESULT.ToString(),
                tooltip,
                NotificationType.MessageImportant);
        }

        internal static void ShowBombardmentPaused(int worldId)
        {
            BuildingComplete focus = MeteorDigService.GetFirstSolarPanel(worldId);
            if (focus == null)
                return;

            Add(
                focus,
                STRINGS.UI.NOTIFICATIONS.BOMBARDMENT_PAUSED.ToString(),
                STRINGS.UI.NOTIFICATIONS.BOMBARDMENT_PAUSED_TOOLTIP.ToString(),
                NotificationType.Neutral);
        }

        internal static void ShowShowerEnded(int worldId)
        {
            BuildingComplete focus = MeteorDigService.GetFirstSolarPanel(worldId);
            if (focus == null)
                return;

            Add(
                focus,
                STRINGS.UI.NOTIFICATIONS.SHOWER_ENDED.ToString(),
                STRINGS.UI.NOTIFICATIONS.SHOWER_ENDED_TOOLTIP.ToString(),
                NotificationType.Neutral);
        }

        private static void Add(BuildingComplete focus, string title, string tooltip, NotificationType type)
        {
            if (focus == null)
                return;

            int worldId = focus.GetMyWorldId();
            Vector3 position = focus.transform.GetPosition();
            Notification notification = new Notification(
                title,
                type,
                (List<Notification> _, object __) => tooltip,
                expires: true,
                custom_click_callback: _ => GameUtil.FocusCameraOnWorld(worldId, position),
                click_focus: focus.transform,
                clear_on_click: true);
            focus.gameObject.AddOrGet<Notifier>().Add(notification);
        }
    }
}
