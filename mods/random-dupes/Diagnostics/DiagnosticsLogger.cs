using RandomDupes.Configuration;
using RandomDupes.Randomization;
using UnityEngine;

namespace RandomDupes.Diagnostics
{
    internal static class DiagnosticsLogger
    {
        internal static void Candidate(MinionStartingStats stats, RandomizedAppearance appearance)
        {
            if (!SettingsResolver.Current().DetailedDiagnostics || stats?.personality == null || appearance == null)
                return;

            KCompBuilder.BodyData data = appearance.BodyData;
            Debug.Log(
                $"[RandomDupes] Candidate original={stats.personality.Id} model={stats.personality.model} " +
                $"displayedName='{stats.Name}' nameDonor={appearance.NameDonorId} " +
                $"hairDonor={appearance.HairDonorId} faceDonor={appearance.FaceDonorId} skinDonor={appearance.SkinDonorId} " +
                $"hair={Symbol(data.hair)} eyes={Symbol(data.eyes)} mouth={Symbol(data.mouth)} " +
                $"head={Symbol(data.headShape)} armLowerSkin={Symbol(data.armLowerSkin)} " +
                $"armUpperSkin={Symbol(data.armUpperSkin)} legSkin={Symbol(data.legSkin)}");
        }

        private static string Symbol(HashedString value)
        {
            return HashCache.Get().Get(value) ?? value.ToString();
        }
    }
}
