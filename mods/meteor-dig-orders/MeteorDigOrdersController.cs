using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace CLD01_MeteorDigOrders
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public sealed class MeteorDigOrdersController : KMonoBehaviour, ISaveLoadable
    {
        private const float InitialScanDelaySeconds = 900f;

        private readonly HashSet<int> eligibleBombardmentWorlds = new HashSet<int>();
        private readonly HashSet<int> unstableWatchWorlds = new HashSet<int>();
        private SchedulerHandle initialScanHandle;

        [Serialize]
        private bool initialScanCompleted;

        internal static MeteorDigOrdersController Instance { get; private set; }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Instance = this;

            if (Game.Instance != null)
            {
                Game.Instance.Subscribe((int)GameHashes.MeteorShowerBombardStateBegins, OnBombardmentStarted);
                Game.Instance.Subscribe((int)GameHashes.MeteorShowerBombardStateEnds, OnBombardmentEnded);
            }

            MeteorDigOrdersOptions options = MeteorDigOrdersOptions.Load();
            if (!initialScanCompleted && options.SolarPanelsEnabled && GameScheduler.Instance != null)
            {
                initialScanHandle = GameScheduler.Instance.Schedule(
                    "CLD01_MeteorDigOrders.InitialScan",
                    InitialScanDelaySeconds,
                    RunInitialScan);
            }
        }

        protected override void OnCleanUp()
        {
            if (Game.Instance != null)
            {
                Game.Instance.Unsubscribe((int)GameHashes.MeteorShowerBombardStateBegins, OnBombardmentStarted);
                Game.Instance.Unsubscribe((int)GameHashes.MeteorShowerBombardStateEnds, OnBombardmentEnded);
            }

            if (initialScanHandle.IsValid)
                initialScanHandle.ClearScheduler();

            eligibleBombardmentWorlds.Clear();
            unstableWatchWorlds.Clear();
            if (Instance == this)
                Instance = null;
            base.OnCleanUp();
        }

        internal bool IsWatchingUnstableWorld(int worldId)
        {
            return unstableWatchWorlds.Contains(worldId);
        }

        internal void ProcessSettledUnstableCell(int worldId, int cell)
        {
            if (!unstableWatchWorlds.Contains(worldId))
                return;

            MeteorDigOrdersOptions options = MeteorDigOrdersOptions.Load();
            if (!options.SolarPanelsEnabled)
                return;

            MeteorDigService.TryProcessCell(worldId, cell, options.ToPrioritySetting());
        }

        internal void StopWatchingUnstableWorld(int worldId)
        {
            unstableWatchWorlds.Remove(worldId);
        }

        internal void ProcessLastMeteorCleanup(int worldId)
        {
            if (!unstableWatchWorlds.Contains(worldId) || Components.Meteors.GetItems(worldId).Count > 0)
                return;

            MeteorDigOrdersOptions options = MeteorDigOrdersOptions.Load();
            if (!options.SolarPanelsEnabled)
                return;

            DigScanResult result = MeteorDigService.ScanWorld(worldId, options.ToPrioritySetting());
            if ((result.Created > 0 || result.Raised > 0) && options.Notifications != NotificationOption.Disabled)
                MeteorDigNotifications.ShowResult(worldId, result);
        }

        internal void NotifyShowerEnded(int worldId)
        {
            MeteorDigOrdersOptions options = MeteorDigOrdersOptions.Load();
            if (options.SolarPanelsEnabled && options.Notifications == NotificationOption.All)
                MeteorDigNotifications.ShowShowerEnded(worldId);
        }

        private void OnBombardmentStarted(object data)
        {
            if (!(data is int worldId))
                return;

            MeteorDigOrdersOptions options = MeteorDigOrdersOptions.Load();
            if (!options.SolarPanelsEnabled)
                return;

            bool eligible = options.WorldScope == WorldScopeOption.AllWorlds ||
                            (ClusterManager.Instance != null && ClusterManager.Instance.activeWorldId == worldId);
            if (!eligible)
                return;

            eligibleBombardmentWorlds.Add(worldId);
            if (options.Notifications == NotificationOption.All)
                MeteorDigNotifications.ShowBombardmentStarted(worldId);
        }

        private void OnBombardmentEnded(object data)
        {
            if (!(data is int worldId) || !eligibleBombardmentWorlds.Remove(worldId))
                return;

            MeteorDigOrdersOptions options = MeteorDigOrdersOptions.Load();
            if (!options.SolarPanelsEnabled)
                return;

            CompleteInitialScanWithoutTimer();
            DigScanResult result = MeteorDigService.ScanWorld(worldId, options.ToPrioritySetting());
            unstableWatchWorlds.Add(worldId);

            if (options.Notifications == NotificationOption.All)
                MeteorDigNotifications.ShowBombardmentPaused(worldId);
            if (options.Notifications != NotificationOption.Disabled)
                MeteorDigNotifications.ShowResult(worldId, result);
        }

        private void RunInitialScan(object data)
        {
            if (initialScanCompleted)
                return;

            initialScanCompleted = true;
            initialScanHandle = default;

            MeteorDigOrdersOptions options = MeteorDigOrdersOptions.Load();
            if (!options.SolarPanelsEnabled || ClusterManager.Instance == null)
                return;

            int worldId = ClusterManager.Instance.activeWorldId;
            DigScanResult result = MeteorDigService.ScanWorld(worldId, options.ToPrioritySetting());
            if (options.Notifications != NotificationOption.Disabled)
                MeteorDigNotifications.ShowResult(worldId, result);
        }

        private void CompleteInitialScanWithoutTimer()
        {
            if (initialScanCompleted)
                return;

            initialScanCompleted = true;
            if (initialScanHandle.IsValid)
                initialScanHandle.ClearScheduler();
        }
    }
}
