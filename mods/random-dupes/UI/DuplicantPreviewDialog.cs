using System;
using System.Collections.Generic;
using Database;
using PeterHan.PLib.UI;
using RandomDupes.Diagnostics;
using RandomDupes.Randomization;
using UnityEngine;

namespace RandomDupes.UI
{
    internal static class DuplicantPreviewDialog
    {
        private sealed class PreviewState
        {
            internal UIMinion Minion;
            internal LocText Details;
        }

        internal static void Show()
        {
            try
            {
                PreviewState state = new PreviewState();
                PSpacer preview = new PSpacer
                {
                    PreferredSize = new Vector2(260f, 310f),
                    FlexSize = Vector2.zero
                };
                preview.OnRealize += host => CreatePreview(host, state);

                PLabel details = new PLabel("Details")
                {
                    Text = "Loading available duplicants...",
                    TextAlignment = TextAnchor.MiddleCenter,
                    FlexSize = new Vector2(1f, 0f)
                };
                details.OnRealize += gameObject => state.Details = gameObject.GetComponent<LocText>();

                PButton randomize = new PButton("Randomize")
                {
                    Text = "Randomize",
                    ToolTip = "Generates another physical combination without changing any save or Printing Pod candidate.",
                    OnClick = _ => Randomize(state)
                }.SetKleiPinkStyle();

                PDialog dialog = new PDialog("RandomDupesPreview")
                {
                    Title = "Random Dupes — Physical Appearance Preview",
                    Size = new Vector2(440f, 520f),
                    MaxSize = new Vector2(600f, 700f)
                };
                dialog.Body.Direction = PanelDirection.Vertical;
                dialog.Body.Spacing = 8;
                dialog.Body.AddChild(preview).AddChild(details).AddChild(randomize);
                dialog.AddButton(PDialog.DIALOG_KEY_CLOSE, "Close", "Close the preview");
                dialog.Show();
            }
            catch (Exception exception)
            {
                ErrorLogger.Write("Opening the duplicant preview", exception);
            }
        }

        private static void CreatePreview(GameObject host, PreviewState state)
        {
            state.Minion = host.AddComponent<UIMinion>();
            state.Minion.TrySpawn();
            Randomize(state);
        }

        private static void Randomize(PreviewState state)
        {
            try
            {
                List<Personality> available = Db.Get().Personalities.GetAll(true, false);
                if (available.Count == 0 || state.Minion == null)
                    return;

                Personality original = available[UnityEngine.Random.Range(0, available.Count)];
                state.Minion.SetMinion(original);

                if (DupeAppearanceRandomizer.TryComposePreview(
                    original,
                    out string displayName,
                    out RandomizedAppearance appearance))
                {
                    Accessorizer accessorizer = state.Minion.SpawnedAvatar.GetComponent<Accessorizer>();
                    accessorizer.ApplyBodyData(appearance.BodyData);
                    state.Minion.React(UIMinionOrMannequinReactSource.OnPersonalityChanged);
                    if (state.Details != null)
                    {
                        state.Details.SetText(
                            $"Original: {original.Name} ({original.Id})  |  Model: {original.model}\n" +
                            $"Name: {displayName}  |  Hair: {appearance.HairDonorId}  |  " +
                            $"Eyes/Mouth: {appearance.FaceDonorId}  |  Skin: {appearance.SkinDonorId}");
                    }
                }
                else if (state.Details != null)
                    state.Details.SetText($"{original.Name} ({original.Id}) — randomization disabled for model {original.model}.");
            }
            catch (Exception exception)
            {
                ErrorLogger.Write("Randomizing the duplicant settings preview", exception);
                if (state.Details != null)
                    state.Details.SetText("Preview failed. See random-dupes-errors.log for details.");
            }
        }
    }
}
