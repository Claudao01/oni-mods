namespace CLD01_MeteorDigOrders
{
    public static class STRINGS
    {
        public static class UI
        {
            public static class ENUMS
            {
                public static LocString URGENT = "Urgent";
                public static LocString ACTIVE_WORLD_ONLY = "Active World Only";
                public static LocString ALL_WORLDS = "All Worlds";
                public static LocString DISABLED = "Disabled";
                public static LocString RESULTS_ONLY = "Results Only";
                public static LocString ALL_NOTIFICATIONS = "Warnings and Results";
            }

            public static class OPTIONS
            {
                public static class DIG_PRIORITY
                {
                    public static LocString NAME = "Dig priority";
                    public static LocString TOOLTIP = "Priority assigned to new dig errands. Existing errands are only raised when their priority is lower.";
                }

                public static class SOLAR_PANELS_ENABLED
                {
                    public static LocString NAME = "Solar panels";
                    public static LocString TOOLTIP = "Automatically queue dig errands for natural terrain overlapping solar panels.";
                }

                public static class WORLD_SCOPE
                {
                    public static LocString NAME = "World scope";
                    public static LocString TOOLTIP = "Active World Only processes a bombardment only when it begins on the viewed world. All Worlds processes every affected asteroid.";
                }

                public static class NOTIFICATIONS
                {
                    public static LocString NAME = "Notifications";
                    public static LocString TOOLTIP = "Choose whether to disable notifications, show only cleanup results, or show meteor warnings and results.";
                }
            }

            public static class NOTIFICATIONS
            {
                public static LocString BOMBARDMENT_STARTED = "Meteor shower in progress";
                public static LocString BOMBARDMENT_STARTED_TOOLTIP = "Meteor activity has begun in this world. Selected buildings will be checked when this bombardment phase ends.";
                public static LocString BOMBARDMENT_PAUSED = "Meteor activity paused";
                public static LocString BOMBARDMENT_PAUSED_TOOLTIP = "This bombardment phase has ended, but more meteor activity may follow.";
                public static LocString CLEANUP_RESULT = "Meteor cleanup queued";
                public static LocString CLEANUP_RESULT_TOOLTIP = "{0} new dig errands\n{1} priorities raised";
                public static LocString NO_CLEANUP_REQUIRED = "No meteor cleanup required";
                public static LocString NO_CLEANUP_REQUIRED_TOOLTIP = "No dig errands were needed for the selected buildings.";
                public static LocString SHOWER_ENDED = "Meteor shower ended";
                public static LocString SHOWER_ENDED_TOOLTIP = "Meteor activity has ended in this world.";
            }
        }
    }
}
