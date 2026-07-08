using System.Collections.Generic;
using System.Linq;
using FillBackgroundPanels;
using UnityEngine;

namespace FillBackgroundPanels.Tools
{
    // Ferramenta "Balde de Tinta" para paredes de fundo. Um clique dentro de uma
    // cavidade fechada preenche as celulas vazias com ordens de construcao e
    // atualiza instantaneamente a skin das paredes de fundo compativeis ja
    // construidas, sem gerar ordem de desconstrucao para elas.
    //
    // Selecao de predio / material / skin: use a roda do mouse sobre o cursor
    // enquanto a ferramenta estiver ativa (sem modificador = predio,
    // Shift = material, Ctrl = skin/facade). A selecao atual e exibida junto ao
    // cursor via PopFXManager.
    public class FillBackgroundTool : InterfaceTool
    {
        public const string ToolName = "FillBackgroundTool";

        public static FillBackgroundTool Instance;

        private List<BuildingDef> availableDefs;
        private int defIndex = -1;

        private List<Tag> availableMaterials;
        private int materialIndex;

        private List<string> availableFacadeIds;
        private int facadeIndex;

        private GameObject previewInstance;

        private BuildingDef SelectedDef =>
            availableDefs != null && defIndex >= 0 && defIndex < availableDefs.Count
                ? availableDefs[defIndex]
                : null;

        private Tag SelectedMaterial =>
            availableMaterials != null && materialIndex >= 0 && materialIndex < availableMaterials.Count
                ? availableMaterials[materialIndex]
                : default;

        private string SelectedFacadeId =>
            availableFacadeIds != null && facadeIndex >= 0 && facadeIndex < availableFacadeIds.Count
                ? availableFacadeIds[facadeIndex]
                : "DEFAULT_FACADE";

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
        }

        protected override void OnActivateTool()
        {
            base.OnActivateTool();
            EnsureSelectionInitialized();
            RebuildPreviewInstance();
            ShowSelectionFeedback();
        }

        protected override void OnDeactivateTool(InterfaceTool new_tool)
        {
            DestroyPreviewInstance();
            base.OnDeactivateTool(new_tool);
        }

        private void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Approximately(scroll, 0f))
                return;

            bool forward = scroll > 0f;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                CycleFacade(forward);
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                CycleMaterial(forward);
            else
                CycleBuildingDef(forward);
        }

        public override void OnLeftClickDown(Vector3 cursor_pos)
        {
            base.OnLeftClickDown(cursor_pos);

            if (SelectedDef == null)
            {
                ShowWarning(cursor_pos, "Nenhuma parede de fundo disponivel para preencher.");
                return;
            }

            int cell = Grid.PosToCell(cursor_pos);
            TryFillCavityAt(cell, cursor_pos);
        }

        private void EnsureSelectionInitialized()
        {
            if (availableDefs == null)
            {
                availableDefs = Assets.BuildingDefs
                    .Where(def => def != null
                        && def.ObjectLayer == ObjectLayer.Backwall
                        && def.ReplacementLayer != ObjectLayer.NumLayers
                        && def.BuildingPreview != null)
                    .OrderBy(def => def.PrefabID)
                    .ToList();
                defIndex = availableDefs.Count > 0 ? 0 : -1;
            }

            RefreshMaterialsForSelectedDef();
            RefreshFacadesForSelectedDef();
        }

        private void RefreshMaterialsForSelectedDef()
        {
            availableMaterials = null;
            materialIndex = 0;

            BuildingDef def = SelectedDef;
            if (def == null || def.MaterialCategory == null || def.MaterialCategory.Length == 0)
                return;

            availableMaterials = MaterialSelector.GetValidMaterials((Tag)def.MaterialCategory[0]);
            if (availableMaterials == null || availableMaterials.Count == 0)
                availableMaterials = def.DefaultElements();
        }

        private void RefreshFacadesForSelectedDef()
        {
            availableFacadeIds = new List<string> { "DEFAULT_FACADE" };
            facadeIndex = 0;

            BuildingDef def = SelectedDef;
            if (def?.AvailableFacades == null)
                return;

            availableFacadeIds.AddRange(def.AvailableFacades
                .Where(id => !string.IsNullOrEmpty(id) && id != "DEFAULT_FACADE"));
        }

        private static int Wrap(int index, int count)
        {
            if (count <= 0)
                return -1;
            return ((index % count) + count) % count;
        }

        private void CycleBuildingDef(bool forward)
        {
            if (availableDefs == null || availableDefs.Count == 0)
                return;

            defIndex = Wrap(defIndex + (forward ? 1 : -1), availableDefs.Count);
            RefreshMaterialsForSelectedDef();
            RefreshFacadesForSelectedDef();
            RebuildPreviewInstance();
            ShowSelectionFeedback();
        }

        private void CycleMaterial(bool forward)
        {
            if (availableMaterials == null || availableMaterials.Count == 0)
                return;

            materialIndex = Wrap(materialIndex + (forward ? 1 : -1), availableMaterials.Count);
            ShowSelectionFeedback();
        }

        private void CycleFacade(bool forward)
        {
            if (availableFacadeIds == null || availableFacadeIds.Count == 0)
                return;

            facadeIndex = Wrap(facadeIndex + (forward ? 1 : -1), availableFacadeIds.Count);
            ShowSelectionFeedback();
        }

        private void RebuildPreviewInstance()
        {
            DestroyPreviewInstance();

            BuildingDef def = SelectedDef;
            if (def == null)
                return;

            Vector3 cursorPos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
            Vector3 pos = Grid.CellToPosCBC(Grid.PosToCell(cursorPos), def.SceneLayer);
            previewInstance = GameUtil.KInstantiate(def.BuildingPreview, pos, def.SceneLayer);
            previewInstance.SetActive(true);

            foreach (Renderer renderer in previewInstance.GetComponentsInChildren<Renderer>(true))
                renderer.enabled = false;

            KBatchedAnimController animController = previewInstance.GetComponent<KBatchedAnimController>();
            if (animController != null)
                animController.enabled = false;
        }

        private void DestroyPreviewInstance()
        {
            if (previewInstance == null)
                return;

            Object.Destroy(previewInstance);
            previewInstance = null;
        }

        private void TryFillCavityAt(int cell, Vector3 feedbackPos)
        {
            if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
                return;

            CavityInfo cavity = Game.Instance.roomProber.GetCavityForCell(cell);
            if (cavity == null)
            {
                ShowWarning(feedbackPos, "Selecione uma celula dentro de uma cavidade fechada.");
                return;
            }

            int maxCavitySize = FillBackgroundConfig.Instance.MaxCavitySize;
            if (cavity.NumCells > maxCavitySize)
            {
                ShowWarning(feedbackPos,
                    $"Cavidade muito grande ({cavity.NumCells} celulas). Limite atual: {maxCavitySize}.");
                return;
            }

            BuildingDef def = SelectedDef;
            if (def == null || availableMaterials == null || availableMaterials.Count == 0)
            {
                ShowWarning(feedbackPos, "Nenhum material disponivel para o predio selecionado.");
                return;
            }

            Tag material = SelectedMaterial;
            string facadeId = SelectedFacadeId;
            IList<Tag> selectedElements = new List<Tag> { material };
            Orientation orientation = def.InitialOrientation;

            int queued = 0;
            int updated = 0;
            int skipped = 0;

            foreach (int targetCell in cavity.cells)
                ProcessCell(targetCell, def, material, facadeId, selectedElements, orientation, ref queued, ref updated, ref skipped);

            bool didSomething = queued > 0 || updated > 0;
            KMonoBehaviour.PlaySound(GlobalAssets.GetSound(didSomething ? "Tile_Confirm" : "Tile_Cancel"));

            string summary = $"{queued} ordens enfileiradas, {updated} skins atualizadas, {skipped} celulas ignoradas.";
            PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, summary, null, feedbackPos, 2f);
        }

        private void ProcessCell(
            int cell,
            BuildingDef def,
            Tag material,
            string facadeId,
            IList<Tag> selectedElements,
            Orientation orientation,
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
            if (previewInstance != null)
                previewInstance.transform.SetPosition(pos);

            GameObject existing = def.GetReplacementCandidate(cell);

            if (existing == null)
            {
                if (def.IsReplacementLayerOccupied(cell))
                {
                    skipped++;
                    return;
                }

                GameObject built = def.TryPlace(previewInstance, pos, orientation, selectedElements, facadeId);
                if (built != null)
                    queued++;
                else
                    skipped++;
                return;
            }

            BuildingComplete existingComplete = existing.GetComponent<BuildingComplete>();
            if (existingComplete == null || !existingComplete.Def.Replaceable || !def.CanReplace(existing))
            {
                skipped++;
                return;
            }

            PrimaryElement existingPrimaryElement = existing.GetComponent<PrimaryElement>();
            Tag existingMaterial = existingPrimaryElement != null && existingPrimaryElement.Element != null
                ? existingPrimaryElement.Element.tag
                : default;

            bool sameBuilding = existingComplete.Def == def;
            bool sameMaterial = existingMaterial == material;

            if (sameBuilding && sameMaterial)
            {
                BuildingFacade facade = existing.GetComponent<BuildingFacade>();
                if (facade == null)
                {
                    skipped++;
                    return;
                }

                bool alreadyApplied = facade.CurrentFacade == facadeId
                    || (facade.IsOriginal && facadeId == "DEFAULT_FACADE");
                if (alreadyApplied)
                {
                    skipped++;
                    return;
                }

                if (string.IsNullOrEmpty(facadeId) || facadeId == "DEFAULT_FACADE")
                    facade.ApplyDefaultFacade();
                else
                    facade.ApplyBuildingFacade(Db.GetBuildingFacades().Get(facadeId));

                updated++;
                return;
            }

            GameObject replaced = def.TryReplaceTile(previewInstance, pos, orientation, selectedElements, facadeId);
            if (replaced != null)
                queued++;
            else
                skipped++;
        }

        private void ShowSelectionFeedback()
        {
            Vector3 cursorPos = PlayerController.GetCursorPos(KInputManager.GetMousePos());

            BuildingDef def = SelectedDef;
            if (def == null)
            {
                ShowWarning(cursorPos, "Nenhuma parede de fundo disponivel neste save.");
                return;
            }

            string materialName = availableMaterials != null && materialIndex < availableMaterials.Count
                ? availableMaterials[materialIndex].ProperName()
                : "?";
            string facadeName = SelectedFacadeId == "DEFAULT_FACADE" ? "Padrao" : SelectedFacadeId;

            string message = $"{def.PrefabID} / {materialName} / {facadeName}";
            PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, message, null, cursorPos, 2f);
        }

        private void ShowWarning(Vector3 pos, string message)
        {
            KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Tile_Cancel"));
            PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, message, null, pos, 2f);
        }
    }
}
