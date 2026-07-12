using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace CLD01_MeteorDigOrders
{
    public enum DigPriorityOption
    {
        [Option("1")]
        Priority1 = 1,
        [Option("2")]
        Priority2 = 2,
        [Option("3")]
        Priority3 = 3,
        [Option("4")]
        Priority4 = 4,
        [Option("5")]
        Priority5 = 5,
        [Option("6")]
        Priority6 = 6,
        [Option("7")]
        Priority7 = 7,
        [Option("8")]
        Priority8 = 8,
        [Option("9")]
        Priority9 = 9,
        [Option("CLD01_MeteorDigOrders.STRINGS.UI.ENUMS.URGENT")]
        Urgent = 10
    }

    public enum WorldScopeOption
    {
        [Option("CLD01_MeteorDigOrders.STRINGS.UI.ENUMS.ACTIVE_WORLD_ONLY")]
        ActiveWorldOnly = 0,
        [Option("CLD01_MeteorDigOrders.STRINGS.UI.ENUMS.ALL_WORLDS")]
        AllWorlds = 1
    }

    public enum NotificationOption
    {
        [Option("CLD01_MeteorDigOrders.STRINGS.UI.ENUMS.DISABLED")]
        Disabled = 0,
        [Option("CLD01_MeteorDigOrders.STRINGS.UI.ENUMS.RESULTS_ONLY")]
        ResultsOnly = 1,
        [Option("CLD01_MeteorDigOrders.STRINGS.UI.ENUMS.ALL_NOTIFICATIONS")]
        All = 2
    }

    [ConfigFile("config.json", true, false)]
    [ModInfo("https://github.com/Claudao01/oni-mods/tree/main/mods/meteor-dig-orders")]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class MeteorDigOrdersOptions
    {
        [JsonProperty]
        [Option(
            "CLD01_MeteorDigOrders.STRINGS.UI.OPTIONS.DIG_PRIORITY.NAME",
            "CLD01_MeteorDigOrders.STRINGS.UI.OPTIONS.DIG_PRIORITY.TOOLTIP")]
        public DigPriorityOption DigPriority { get; set; } = DigPriorityOption.Priority5;

        [JsonProperty]
        [Option(
            "CLD01_MeteorDigOrders.STRINGS.UI.OPTIONS.SOLAR_PANELS_ENABLED.NAME",
            "CLD01_MeteorDigOrders.STRINGS.UI.OPTIONS.SOLAR_PANELS_ENABLED.TOOLTIP")]
        public bool SolarPanelsEnabled { get; set; } = true;

        [JsonProperty]
        [Option(
            "CLD01_MeteorDigOrders.STRINGS.UI.OPTIONS.WORLD_SCOPE.NAME",
            "CLD01_MeteorDigOrders.STRINGS.UI.OPTIONS.WORLD_SCOPE.TOOLTIP")]
        public WorldScopeOption WorldScope { get; set; } = WorldScopeOption.ActiveWorldOnly;

        [JsonProperty]
        [Option(
            "CLD01_MeteorDigOrders.STRINGS.UI.OPTIONS.NOTIFICATIONS.NAME",
            "CLD01_MeteorDigOrders.STRINGS.UI.OPTIONS.NOTIFICATIONS.TOOLTIP")]
        public NotificationOption Notifications { get; set; } = NotificationOption.ResultsOnly;

        public static MeteorDigOrdersOptions Load()
        {
            return POptions.ReadSettings<MeteorDigOrdersOptions>() ?? new MeteorDigOrdersOptions();
        }

        public PrioritySetting ToPrioritySetting()
        {
            return DigPriority == DigPriorityOption.Urgent
                ? new PrioritySetting(PriorityScreen.PriorityClass.topPriority, 1)
                : new PrioritySetting(PriorityScreen.PriorityClass.basic, (int)DigPriority);
        }
    }
}
