using KSerialization;
using Newtonsoft.Json;
using UnityEngine;

namespace RandomDupes.Configuration
{
    [SerializationConfig(KSerialization.MemberSerialization.OptIn)]
    public sealed class RandomDupesSaveSettings : KMonoBehaviour, ISaveLoadable
    {
        [Serialize]
        private string settingsJson;

        [Serialize]
        private int deterministicSequence;

        [Serialize]
        private bool hasOverride;

        internal static RandomDupesSaveSettings Instance { get; private set; }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Instance = this;
        }

        protected override void OnCleanUp()
        {
            if (Instance == this)
                Instance = null;
            base.OnCleanUp();
        }

        internal RandomDupesOptions Read()
        {
            try
            {
                return JsonConvert.DeserializeObject<RandomDupesOptions>(settingsJson) ?? RandomDupesOptions.Load();
            }
            catch (System.Exception exception)
            {
                Diagnostics.ErrorLogger.Write("Reading save-specific settings", exception);
                return RandomDupesOptions.Load();
            }
        }

        internal bool HasOverride => hasOverride && !string.IsNullOrEmpty(settingsJson);

        internal int NextSequence()
        {
            return deterministicSequence++;
        }

        internal static RandomDupesOptions ReadCurrentOrGlobal()
        {
            if (Instance == null)
                return RandomDupesOptions.Load();
            return Instance.HasOverride ? Instance.Read() : RandomDupesOptions.Load();
        }

        internal static bool ApplyOverride(RandomDupesOptions options)
        {
            if (Instance == null || options == null)
                return false;
            Instance.Capture(options);
            return true;
        }

        internal static bool ClearOverride()
        {
            if (Instance == null)
                return false;
            Instance.settingsJson = null;
            Instance.deterministicSequence = 0;
            Instance.hasOverride = false;
            return true;
        }

        private void Capture(RandomDupesOptions options)
        {
            settingsJson = JsonConvert.SerializeObject(options);
            deterministicSequence = 0;
            hasOverride = true;
        }
    }

    internal static class SettingsResolver
    {
        private static int globalSequence;

        internal static RandomDupesOptions Current()
        {
            RandomDupesOptions global = RandomDupesOptions.Load();
            return RandomDupesSaveSettings.Instance != null && RandomDupesSaveSettings.Instance.HasOverride
                ? RandomDupesSaveSettings.Instance.Read().Effective()
                : global.Effective();
        }

        internal static int NextSequence()
        {
            return RandomDupesSaveSettings.Instance != null
                ? RandomDupesSaveSettings.Instance.NextSequence()
                : globalSequence++;
        }
    }
}
