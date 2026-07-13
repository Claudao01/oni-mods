using System;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace RandomDupes.Configuration
{
    public enum RandomizationProfile
    {
        [Option("Subtle")]
        Subtle,
        [Option("Full physical appearance")]
        FullPhysical,
        [Option("Controlled chaos")]
        ControlledChaos,
        [Option("Custom")]
        Custom
    }

    [ConfigFile("config.json", true, false)]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RandomDupesOptions
    {
        [JsonProperty, Option("Profile", "Profiles provide useful defaults. Custom uses every toggle below.", "General")]
        public RandomizationProfile Profile { get; set; } = RandomizationProfile.FullPhysical;

        [JsonProperty, Option("Randomize names", "Changes only the displayed name; personality and biography remain original.", "Physical appearance")]
        public bool RandomizeNames { get; set; } = true;

        [JsonProperty, Option("Match name gender", "Name donors must use the same gender key as the original personality.", "Physical appearance")]
        public bool MatchNameGender { get; set; } = true;

        [JsonProperty, Option("Randomize hair", "Randomizes hair and its matching hat-hair symbol.", "Physical appearance")]
        public bool RandomizeHair { get; set; } = true;

        [JsonProperty, Option("Randomize eyes", "Randomizes the physical eye symbols.", "Physical appearance")]
        public bool RandomizeEyes { get; set; } = true;

        [JsonProperty, Option("Randomize mouth", "Randomizes the physical mouth symbols.", "Physical appearance")]
        public bool RandomizeMouth { get; set; } = true;

        [JsonProperty, Option("Randomize skin set", "Keeps head shape, arm skin and leg skin from one coherent donor.", "Physical appearance")]
        public bool RandomizeSkin { get; set; } = true;

        [JsonProperty, Option("Standard duplicants", "Allows physical randomization for Standard duplicants.", "Models")]
        public bool EnableStandard { get; set; } = true;

        [JsonProperty, Option("Bionic duplicants", "Allows physical randomization for Bionic duplicants without mixing models.", "Models")]
        public bool EnableBionic { get; set; } = true;

        [JsonProperty, Option("Appearance reroll button", "Adds a button to reroll only physical appearance in the Printing Pod.", "Interface")]
        public bool EnablePrinterAppearanceReroll { get; set; } = true;

        [JsonProperty, Option("Deterministic seed", "Uses Seed and a serialized sequence for repeatable random choices.", "Randomization")]
        public bool DeterministicSeed { get; set; } = false;

        [JsonProperty, Option("Seed", "Seed used when deterministic mode is enabled.", "Randomization")]
        public int Seed { get; set; } = 1024;

        [JsonProperty, Option("Avoid repeated donors", "Tries to use a different personality for each randomized part.", "Randomization")]
        public bool AvoidRepeatedDonors { get; set; } = true;

        [JsonProperty, Option("Original appearance weight (%)", "Chance for each physical part to remain original.", "Randomization")]
        [Limit(0, 100)]
        public int OriginalWeightPercent { get; set; } = 10;

        [JsonProperty, Option("Excluded donor IDs", "Comma-separated personality IDs excluded from donor pools.", "Randomization")]
        public string ExcludedDonorIds { get; set; } = string.Empty;

        [JsonProperty, Option("Detailed diagnostics", "Writes donor IDs and final symbols to Player.log. Error logs are always enabled.", "Diagnostics")]
        public bool DetailedDiagnostics { get; set; } = false;

        [JsonIgnore, Option("Export available duplicants", "Writes a CSV catalog of every currently loaded personality.", "Diagnostics")]
        public Action<object> ExportCatalog => _ => Diagnostics.PersonalityCatalogExporter.Export();

        [JsonIgnore, Option("Duplicant preview", "Opens an animated duplicant viewer with a Randomize button.", "Interface")]
        public Action<object> OpenPreview => _ => UI.DuplicantPreviewDialog.Show();

        public static RandomDupesOptions Load()
        {
            return POptions.ReadSettings<RandomDupesOptions>() ?? new RandomDupesOptions();
        }

        public RandomDupesOptions Effective()
        {
            RandomDupesOptions copy = Clone();
            switch (Profile)
            {
                case RandomizationProfile.Subtle:
                    copy.RandomizeNames = false;
                    copy.RandomizeHair = true;
                    copy.RandomizeEyes = false;
                    copy.RandomizeMouth = false;
                    copy.RandomizeSkin = false;
                    copy.OriginalWeightPercent = 35;
                    break;
                case RandomizationProfile.FullPhysical:
                    copy.RandomizeNames = true;
                    copy.RandomizeHair = true;
                    copy.RandomizeEyes = true;
                    copy.RandomizeMouth = true;
                    copy.RandomizeSkin = true;
                    copy.OriginalWeightPercent = 10;
                    break;
                case RandomizationProfile.ControlledChaos:
                    copy.RandomizeNames = true;
                    copy.RandomizeHair = true;
                    copy.RandomizeEyes = true;
                    copy.RandomizeMouth = true;
                    copy.RandomizeSkin = true;
                    copy.MatchNameGender = false;
                    copy.AvoidRepeatedDonors = true;
                    copy.OriginalWeightPercent = 0;
                    break;
            }
            return copy;
        }

        public RandomDupesOptions Clone()
        {
            return JsonConvert.DeserializeObject<RandomDupesOptions>(JsonConvert.SerializeObject(this)) ?? new RandomDupesOptions();
        }

    }
}
