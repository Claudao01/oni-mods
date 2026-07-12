using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace FillWithBackwalls
{
    [ConfigFile("config.json", true, false)]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FillWithBackwallsOptions
    {
        public const int DefaultMaxCavitySize = 120;
        public const int MinimumMaxCavitySize = 1;

        private int maxCavitySize = DefaultMaxCavitySize;

        [JsonProperty]
        [Option(
            "FillWithBackwalls.STRINGS.UI.OPTIONS.MAX_CAVITY_SIZE.NAME",
            "FillWithBackwalls.STRINGS.UI.OPTIONS.MAX_CAVITY_SIZE.TOOLTIP")]
        public int MaxCavitySize
        {
            get => maxCavitySize;
            set => maxCavitySize = value < MinimumMaxCavitySize ? MinimumMaxCavitySize : value;
        }

        public static FillWithBackwallsOptions Load()
        {
            return POptions.ReadSettings<FillWithBackwallsOptions>() ?? new FillWithBackwallsOptions();
        }
    }
}
