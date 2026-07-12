namespace FillWithBackwalls
{
    public static class STRINGS
    {
        public static class UI
        {
            public static class FILL_CAVITY
            {
                public static LocString NAME = "Fill cavity";
                public static LocString TOOLTIP = "Fills the entire enclosed cavity with the selected backwall blueprint, material, and facade.";
            }

            public static class OPTIONS
            {
                public static class MAX_CAVITY_SIZE
                {
                    public static LocString NAME = "Maximum cavity size";
                    public static LocString TOOLTIP = "Maximum number of cells that can be filled with one click.";
                }
            }

            public static class FEEDBACK
            {
                public static LocString NO_MATERIAL = "No material is selected for this backwall.";
                public static LocString CLOSED_CAVITY_REQUIRED = "Select a cell inside an enclosed cavity.";
                public static LocString CAVITY_TOO_LARGE = "Cavity too large ({0} cells). Current limit: {1}.";
                public static LocString SUMMARY = "{0} build orders queued, {1} facades updated, {2} cells skipped.";
            }
        }
    }
}
