using System.Collections.Generic;
using UnityEngine;

namespace CLD01_MeteorDigOrders
{
    internal readonly struct DigScanResult
    {
        internal DigScanResult(int created, int raised, BuildingComplete firstAffected)
        {
            Created = created;
            Raised = raised;
            FirstAffected = firstAffected;
        }

        internal int Created { get; }
        internal int Raised { get; }
        internal BuildingComplete FirstAffected { get; }
    }

    internal static class MeteorDigService
    {
        private static readonly Tag SolarPanelTag = new Tag(SolarPanelConfig.ID);

        internal static DigScanResult ScanWorld(int worldId, PrioritySetting desiredPriority)
        {
            int created = 0;
            int raised = 0;
            BuildingComplete firstAffected = null;

            foreach (BuildingComplete building in GetSolarPanels(worldId))
            {
                if (building == null || building.Def == null)
                    continue;

                bool affected = false;
                int[] cells = building.PlacementCells;
                if (cells == null)
                    continue;

                foreach (int cell in cells)
                {
                    if (!IsNaturalDigCell(cell))
                        continue;

                    GameObject existing = Grid.Objects[cell, (int)ObjectLayer.DigPlacer];
                    bool hadOrder = existing != null;
                    GameObject digOrder = DigTool.PlaceDig(cell);
                    if (digOrder == null)
                        continue;

                    Prioritizable prioritizable = digOrder.GetComponent<Prioritizable>();
                    if (prioritizable == null)
                        continue;

                    PrioritySetting current = prioritizable.GetMasterPriority();
                    if (!hadOrder)
                    {
                        prioritizable.SetMasterPriority(desiredPriority);
                        created++;
                        affected = true;
                    }
                    else if (current < desiredPriority)
                    {
                        prioritizable.SetMasterPriority(desiredPriority);
                        raised++;
                        affected = true;
                    }
                }

                if (affected && firstAffected == null)
                    firstAffected = building;
            }

            return new DigScanResult(created, raised, firstAffected);
        }

        internal static bool IsProtectedCell(int worldId, int cell)
        {
            if (!Grid.IsValidCellInWorld(cell, worldId))
                return false;

            foreach (BuildingComplete building in GetSolarPanels(worldId))
            {
                if (building != null && building.PlacementCellsContainCell(cell))
                    return true;
            }

            return false;
        }

        internal static BuildingComplete GetFirstSolarPanel(int worldId)
        {
            foreach (BuildingComplete building in GetSolarPanels(worldId))
                return building;
            return null;
        }

        internal static bool TryProcessCell(int worldId, int cell, PrioritySetting desiredPriority)
        {
            if (!IsProtectedCell(worldId, cell) || !IsNaturalDigCell(cell))
                return false;

            GameObject digOrder = DigTool.PlaceDig(cell);
            if (digOrder == null)
                return false;

            Prioritizable prioritizable = digOrder.GetComponent<Prioritizable>();
            if (prioritizable == null)
                return false;

            if (prioritizable.GetMasterPriority() < desiredPriority)
                prioritizable.SetMasterPriority(desiredPriority);
            return true;
        }

        private static IEnumerable<BuildingComplete> GetSolarPanels(int worldId)
        {
            foreach (BuildingComplete building in Components.BuildingCompletes.WorldItemsEnumerate(worldId, false))
            {
                if (building != null && building.prefabid != null && building.prefabid.PrefabTag == SolarPanelTag)
                    yield return building;
            }
        }

        private static bool IsNaturalDigCell(int cell)
        {
            return Grid.IsValidCell(cell) && Grid.Solid[cell] && !Grid.Foundation[cell];
        }
    }
}
