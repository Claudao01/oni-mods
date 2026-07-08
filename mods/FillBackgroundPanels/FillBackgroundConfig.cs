using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace FillBackgroundPanels
{
    public class FillBackgroundConfig
    {
        public const int DefaultMaxCavitySize = 120;
        public const int MinimumMaxCavitySize = 1;

        private static FillBackgroundConfig instance;

        public static FillBackgroundConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = Load();
                return instance;
            }
        }

        public int MaxCavitySize { get; set; } = DefaultMaxCavitySize;

        private static string ConfigFilePath
        {
            get
            {
                string modDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(modDirectory ?? string.Empty, "config.json");
            }
        }

        private static FillBackgroundConfig Load()
        {
            try
            {
                string path = ConfigFilePath;
                if (File.Exists(path))
                {
                    FillBackgroundConfig loaded = JsonConvert.DeserializeObject<FillBackgroundConfig>(File.ReadAllText(path));
                    if (loaded != null)
                    {
                        loaded.Clamp();
                        return loaded;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[FillBackgroundPanels] Falha ao carregar config.json, usando valores padrao: " + ex.Message);
            }

            FillBackgroundConfig defaults = new FillBackgroundConfig();
            defaults.Save();
            return defaults;
        }

        public void Save()
        {
            try
            {
                Clamp();
                File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[FillBackgroundPanels] Falha ao salvar config.json: " + ex.Message);
            }
        }

        private void Clamp()
        {
            if (MaxCavitySize < MinimumMaxCavitySize)
                MaxCavitySize = MinimumMaxCavitySize;
        }
    }
}
