using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Database;
using RandomDupes.Configuration;
using RandomDupes.Diagnostics;
using UnityEngine;

namespace RandomDupes.Randomization
{
    internal static class DupeAppearanceRandomizer
    {
        private static readonly ConditionalWeakTable<MinionStartingStats, RandomizedAppearance> Results =
            new ConditionalWeakTable<MinionStartingStats, RandomizedAppearance>();

        internal static void Randomize(MinionStartingStats stats, bool starter, bool allowName = true)
        {
            if (stats?.personality == null || Results.TryGetValue(stats, out _))
                return;

            try
            {
                if (!TryCompose(stats.personality, starter, allowName, out string displayName, out RandomizedAppearance result))
                    return;

                if (allowName)
                    stats.Name = displayName;
                Results.Add(stats, result);
                DiagnosticsLogger.Candidate(stats, result);
            }
            catch (Exception exception)
            {
                ErrorLogger.Write("Randomizing a duplicant candidate", exception);
            }
        }

        internal static bool TryComposePreview(
            Personality original,
            out string displayName,
            out RandomizedAppearance appearance)
        {
            try
            {
                return TryCompose(original, false, true, out displayName, out appearance);
            }
            catch (Exception exception)
            {
                displayName = original?.Name ?? string.Empty;
                appearance = null;
                ErrorLogger.Write("Composing a front-end duplicant preview", exception);
                return false;
            }
        }

        internal static void RerollPhysicalAppearance(MinionStartingStats stats)
        {
            if (stats == null)
                return;
            Results.Remove(stats);
            Randomize(stats, false, false);
        }

        internal static bool TryGet(MinionStartingStats stats, out RandomizedAppearance appearance)
        {
            appearance = null;
            return stats != null && Results.TryGetValue(stats, out appearance);
        }

        private static bool TryCompose(
            Personality original,
            bool starter,
            bool allowName,
            out string displayName,
            out RandomizedAppearance appearance)
        {
            displayName = original?.Name ?? string.Empty;
            appearance = null;
            if (original == null)
                return false;

            RandomDupesOptions options = SettingsResolver.Current();
            if (!IsModelEnabled(original.model, options) ||
                !AppearancePools.TryGet(original.model, starter, out List<Personality> sourcePool))
                return false;

            HashSet<string> exclusions = ParseIds(options.ExcludedDonorIds);
            List<Personality> pool = sourcePool.Where(personality => !exclusions.Contains(personality.Id)).ToList();
            if (pool.Count == 0)
                pool.Add(original);

            System.Random deterministic = options.DeterministicSeed
                ? new System.Random(unchecked(options.Seed + SettingsResolver.NextSequence() * 397))
                : null;
            HashSet<string> used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Personality nameDonor = original;
            Personality hairDonor = original;
            Personality eyesDonor = original;
            Personality mouthDonor = original;
            Personality skinDonor = original;

            if (allowName && options.RandomizeNames)
            {
                List<Personality> names = options.MatchNameGender
                    ? pool.Where(item => string.Equals(item.genderStringKey, original.genderStringKey, StringComparison.OrdinalIgnoreCase)).ToList()
                    : pool;
                nameDonor = Pick(names.Count > 0 ? names : pool, original, options, used, deterministic);
                displayName = nameDonor.Name;
            }

            KCompBuilder.BodyData mixed = MinionStartingStats.CreateBodyData(original);
            AccessorySlots slots = Db.Get().AccessorySlots;

            if (options.RandomizeHair)
            {
                hairDonor = Pick(pool, original, options, used, deterministic);
                KCompBuilder.BodyData data = MinionStartingStats.CreateBodyData(hairDonor);
                if (IsValidHair(slots, data.hair))
                    mixed.hair = data.hair;
                else
                    hairDonor = original;
            }

            if (options.RandomizeEyes)
            {
                eyesDonor = Pick(pool, original, options, used, deterministic);
                HashedString eyes = MinionStartingStats.CreateBodyData(eyesDonor).eyes;
                if (slots.Eyes.Lookup(eyes) != null)
                    mixed.eyes = eyes;
                else
                    eyesDonor = original;
            }

            if (options.RandomizeMouth)
            {
                mouthDonor = Pick(pool, original, options, used, deterministic);
                HashedString mouth = MinionStartingStats.CreateBodyData(mouthDonor).mouth;
                if (slots.Mouth.Lookup(mouth) != null)
                    mixed.mouth = mouth;
                else
                    mouthDonor = original;
            }

            if (options.RandomizeSkin)
            {
                skinDonor = Pick(pool, original, options, used, deterministic);
                KCompBuilder.BodyData skin = MinionStartingStats.CreateBodyData(skinDonor);
                if (IsValidSkin(slots, skin))
                {
                    mixed.headShape = skin.headShape;
                    mixed.armLowerSkin = skin.armLowerSkin;
                    mixed.armUpperSkin = skin.armUpperSkin;
                    mixed.legSkin = skin.legSkin;
                }
                else
                    skinDonor = original;
            }

            string faceDonors = eyesDonor.Id == mouthDonor.Id
                ? eyesDonor.Id
                : eyesDonor.Id + "/" + mouthDonor.Id;
            appearance = new RandomizedAppearance(
                mixed, nameDonor.Id, hairDonor.Id, faceDonors, skinDonor.Id);
            return true;
        }

        private static Personality Pick(
            List<Personality> pool,
            Personality original,
            RandomDupesOptions options,
            HashSet<string> used,
            System.Random deterministic)
        {
            if (Roll(100, deterministic) < options.OriginalWeightPercent)
                return original;

            List<Personality> candidates = options.AvoidRepeatedDonors
                ? pool.Where(item => !used.Contains(item.Id)).ToList()
                : pool;
            if (candidates.Count == 0)
                candidates = pool;
            Personality selected = candidates[Roll(candidates.Count, deterministic)];
            used.Add(selected.Id);
            return selected;
        }

        private static int Roll(int maximum, System.Random deterministic)
        {
            return deterministic != null
                ? deterministic.Next(maximum)
                : UnityEngine.Random.Range(0, maximum);
        }

        private static bool IsModelEnabled(Tag model, RandomDupesOptions options)
        {
            if (model == GameTags.Minions.Models.Bionic)
                return options.EnableBionic;
            return model != GameTags.Minions.Models.Standard || options.EnableStandard;
        }

        private static HashSet<string> ParseIds(string value)
        {
            return new HashSet<string>(
                (value ?? string.Empty).Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim()),
                StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsValidHair(AccessorySlots slots, HashedString hair)
        {
            if (slots.Hair.Lookup(hair) == null)
                return false;
            string hairId = HashCache.Get().Get(hair);
            return !string.IsNullOrEmpty(hairId) && slots.HatHair.Lookup("hat_" + hairId) != null;
        }

        private static bool IsValidSkin(AccessorySlots slots, KCompBuilder.BodyData skin)
        {
            return slots.HeadShape.Lookup(skin.headShape) != null &&
                   slots.ArmLowerSkin.Lookup(skin.armLowerSkin) != null &&
                   slots.ArmUpperSkin.Lookup(skin.armUpperSkin) != null &&
                   slots.LegSkin.Lookup(skin.legSkin) != null;
        }
    }
}
