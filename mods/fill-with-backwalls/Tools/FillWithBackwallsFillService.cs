using System.Collections.Generic;
using Database;
using UnityEngine;

namespace FillWithBackwalls.Tools
{
    public static class FillWithBackwallsFillService
    {
        private const string DefaultFacadeId = "DEFAULT_FACADE";

        public static bool TryFillCavityAt(
            int cell,
            BuildingDef def,
            GameObject visualizer,
            Orientation orientation,
            IList<Tag> selectedElements,
            string facadeId,
            Vector3 feedbackPos)
        {
            if (def == null || def.ObjectLayer != ObjectLayer.Backwall)
                return false;

            if (selectedElements == null || selectedElements.Count == 0)
            {
                ShowWarning(feedbackPos, (string)STRINGS.UI.FEEDBACK.NO_MATERIAL);
                return true;
            }

            if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
                return true;

            CavityInfo cavity = Game.Instance?.roomProber?.GetCavityForCell(cell);
            if (cavity == null || cavity.cells == null || cavity.NumCells <= 0)
            {
                ShowWarning(feedbackPos, (string)STRINGS.UI.FEEDBACK.CLOSED_CAVITY_REQUIRED);
                return true;
            }

            int maxCavitySize = FillWithBackwallsOptions.Load().MaxCavitySize;
            if (cavity.NumCells > maxCavitySize)
            {
                ShowWarning(feedbackPos, string.Format((string)STRINGS.UI.FEEDBACK.CAVITY_TOO_LARGE, cavity.NumCells, maxCavitySize));
                return true;
            }

            string safeFacadeId = NormalizeFacadeId(def, facadeId);
            if (PlanScreen.Instance != null)
                PlanScreen.Instance.LastSelectedBuildingFacade = safeFacadeId;

            Tag primaryMaterial = NormalizeMaterial(selectedElements[0]);
            int queued = 0;
            int updated = 0;
            int skipped = 0;

            foreach (int targetCell in cavity.cells)
                ProcessCell(targetCell, def, visualizer, orientation, selectedElements, primaryMaterial, safeFacadeId, ref queued, ref updated, ref skipped);

            bool didSomething = queued > 0 || updated > 0;
            KMonoBehaviour.PlaySound(GlobalAssets.GetSound(didSomething ? "Tile_Confirm" : "Tile_Cancel"));

            string summary = string.Format((string)STRINGS.UI.FEEDBACK.SUMMARY, queued, updated, skipped);
            PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, summary, null, feedbackPos, 2f);
            return true;
        }

        private static void ProcessCell(
            int cell,
            BuildingDef def,
            GameObject visualizer,
            Orientation orientation,
            IList<Tag> selectedElements,
            Tag primaryMaterial,
            string facadeId,
            ref int queued,
            ref int updated,
            ref int skipped)
        {
            if (!Grid.IsValidBuildingCell(cell))
            {
                skipped++;
                return;
            }

            Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
            if (visualizer != null)
                visualizer.transform.SetPosition(pos);

            GameObject existing = def.GetReplacementCandidate(cell);
            if (existing == null)
            {
                if (IsReplacementAreaOccupied(def, cell, orientation))
                {
                    skipped++;
                    return;
                }

                GameObject built = def.TryPlace(visualizer, pos, orientation, selectedElements, facadeId);
                if (built != null)
                    queued++;
                else
                    skipped++;
                return;
            }

            BuildingComplete existingComplete = existing.GetComponent<BuildingComplete>();
            if (existingComplete == null)
            {
                skipped++;
                return;
            }

            Tag existingMaterial = GetPrimaryMaterial(existing);
            bool sameBuilding = existingComplete.Def == def;
            bool sameMaterial = existingMaterial == primaryMaterial;

            if (sameBuilding && sameMaterial)
            {
                if (TryApplyFacade(existing, def, facadeId))
                    updated++;
                else
                    skipped++;
                return;
            }

            if (def.ReplacementLayer == ObjectLayer.NumLayers || !existingComplete.Def.Replaceable || !def.CanReplace(existing) || IsReplacementAreaOccupied(def, cell, orientation))
            {
                skipped++;
                return;
            }

            GameObject replaced = def.TryReplaceTile(visualizer, pos, orientation, selectedElements, facadeId);
            if (replaced != null)
            {
                Grid.Objects[cell, (int)def.ReplacementLayer] = replaced;
                queued++;
            }
            else
            {
                skipped++;
            }
        }

        private static bool IsReplacementAreaOccupied(BuildingDef def, int cell, Orientation orientation)
        {
            if (def.ReplacementLayer == ObjectLayer.NumLayers)
                return false;

            bool occupied = false;
            def.RunOnArea(cell, orientation, offsetCell =>
            {
                if (def.IsReplacementLayerOccupied(offsetCell))
                    occupied = true;
            });
            return occupied;
        }

        private static bool TryApplyFacade(GameObject existing, BuildingDef def, string facadeId)
        {
            BuildingFacade facade = existing.GetComponent<BuildingFacade>();
            if (facade == null)
                return false;

            if (string.IsNullOrEmpty(facadeId) || facadeId == DefaultFacadeId)
            {
                if (facade.CurrentFacade == DefaultFacadeId || facade.IsOriginal)
                    return false;

                facade.ApplyDefaultFacade();
                return true;
            }

            BuildingFacadeResource facadeResource = Db.GetBuildingFacades().TryGet(facadeId);
            if (facadeResource == null || facadeResource.PrefabID != def.PrefabID || !facadeResource.IsUnlocked())
                return false;

            if (facade.CurrentFacade == facadeId)
                return false;

            facade.ApplyBuildingFacade(facadeResource);
            return true;
        }

        private static string NormalizeFacadeId(BuildingDef def, string facadeId)
        {
            if (string.IsNullOrEmpty(facadeId) || facadeId == DefaultFacadeId)
                return DefaultFacadeId;

            BuildingFacadeResource facadeResource = Db.GetBuildingFacades().TryGet(facadeId);
            return facadeResource != null && facadeResource.PrefabID == def.PrefabID && facadeResource.IsUnlocked()
                ? facadeId
                : DefaultFacadeId;
        }

        private static Tag GetPrimaryMaterial(GameObject go)
        {
            PrimaryElement primaryElement = go.GetComponent<PrimaryElement>();
            if (primaryElement == null || primaryElement.Element == null)
                return default;

            return NormalizeMaterial(primaryElement.Element.tag);
        }

        private static Tag NormalizeMaterial(Tag tag)
        {
            return tag.GetHash() == 1542131326 ? SimHashes.Snow.CreateTag() : tag;
        }

        private static void ShowWarning(Vector3 pos, string message)
        {
            KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Tile_Cancel"));
            PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, message, null, pos, 2f);
        }
    }
}
