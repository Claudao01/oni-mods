using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PeterHan.PLib.UI;
using UnityEngine;

namespace PerSaveModSettings.UI
{
    internal static class ModSettingsDialog
    {
        private const string TitleMethod = "GetPerSaveOptionsTitle";
        private const string ShowMethod = "ShowPerSaveOptions";
        private const string ResetMethod = "ResetPerSaveOptions";

        private sealed class Provider
        {
            internal string Title;
            internal MethodInfo Show;
            internal MethodInfo Reset;
        }

        internal static bool HasProviders() => DiscoverProviders().Count > 0;

        internal static void Show()
        {
            try
            {
                List<Provider> providers = DiscoverProviders();
                PPanel list = new PPanel("ModList")
                {
                    Direction = PanelDirection.Vertical,
                    Alignment = TextAnchor.UpperCenter,
                    Spacing = 8,
                    FlexSize = Vector2.right
                };
                foreach (Provider provider in providers)
                    list.AddChild(BuildProviderRow(provider));

                PDialog dialog = new PDialog("PerSaveModSettings")
                {
                    Title = "Mod Settings — Current Save",
                    Size = new Vector2(620f, 420f),
                    MaxSize = new Vector2(760f, 760f),
                    SortKey = 140f
                };
                dialog.Body.Direction = PanelDirection.Vertical;
                dialog.Body.Spacing = 8;
                dialog.Body.AddChild(new PLabel("Description")
                {
                    Text = "Settings listed here override global defaults only for the loaded save.",
                    TextAlignment = TextAnchor.MiddleCenter,
                    FlexSize = Vector2.right
                });
                dialog.Body.AddChild(new PScrollPane("ModListScroll")
                {
                    Child = list,
                    ScrollHorizontal = false,
                    ScrollVertical = true,
                    AlwaysShowVertical = false,
                    TrackSize = 8f,
                    FlexSize = Vector2.one
                });
                dialog.AddButton(PDialog.DIALOG_KEY_CLOSE, "Close", "Return to Options");
                dialog.Show();
            }
            catch (Exception exception)
            {
                Debug.LogError($"[PerSaveModSettings] Could not open the mod list: {exception}");
            }
        }

        private static PPanel BuildProviderRow(Provider provider)
        {
            PPanel row = new PPanel("ModEntry")
            {
                Direction = PanelDirection.Horizontal,
                Alignment = TextAnchor.MiddleCenter,
                Spacing = 8,
                FlexSize = Vector2.right
            };
            row.AddChild(new PLabel("ModTitle")
            {
                Text = provider.Title,
                TextAlignment = TextAnchor.MiddleLeft,
                FlexSize = Vector2.right
            });
            row.AddChild(new PButton("Configure")
            {
                Text = "Configure",
                ToolTip = "Edit settings stored in the current save.",
                OnClick = _ => Invoke(provider.Show)
            }.SetKleiPinkStyle());
            if (provider.Reset != null)
            {
                row.AddChild(new PButton("UseGlobalDefaults")
                {
                    Text = "Use global defaults",
                    ToolTip = "Remove this save override.",
                    OnClick = _ => Invoke(provider.Reset)
                }.SetKleiBlueStyle());
            }
            return row;
        }

        private static List<Provider> DiscoverProviders()
        {
            List<Provider> providers = new List<Provider>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in GetTypesSafely(assembly))
                {
                    MethodInfo title = FindMethod(type, TitleMethod, typeof(string));
                    MethodInfo show = FindMethod(type, ShowMethod, typeof(void));
                    if (title == null || show == null)
                        continue;
                    try
                    {
                        string display = title.Invoke(null, null) as string;
                        if (!string.IsNullOrWhiteSpace(display))
                        {
                            providers.Add(new Provider
                            {
                                Title = display,
                                Show = show,
                                Reset = FindMethod(type, ResetMethod, typeof(void))
                            });
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogWarning($"[PerSaveModSettings] Ignored provider {type.FullName}: {exception.Message}");
                    }
                }
            }
            return providers.OrderBy(provider => provider.Title, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static IEnumerable<Type> GetTypesSafely(Assembly assembly)
        {
            try { return assembly.GetTypes(); }
            catch (ReflectionTypeLoadException exception) { return exception.Types.Where(type => type != null); }
            catch { return Array.Empty<Type>(); }
        }

        private static MethodInfo FindMethod(Type type, string name, Type returnType)
        {
            MethodInfo method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null, Type.EmptyTypes, null);
            return method != null && method.ReturnType == returnType ? method : null;
        }

        private static void Invoke(MethodInfo method)
        {
            try { method?.Invoke(null, null); }
            catch (TargetInvocationException exception)
            {
                Debug.LogError($"[PerSaveModSettings] Provider failed: {exception.InnerException ?? exception}");
            }
            catch (Exception exception) { Debug.LogError($"[PerSaveModSettings] Provider failed: {exception}"); }
        }
    }
}
