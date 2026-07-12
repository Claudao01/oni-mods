using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Database;
using UnityEngine;

namespace RandomDupes.Randomization
{
    internal static class DupeAppearanceRandomizer
    {
        private static readonly ConditionalWeakTable<MinionStartingStats, RandomizedAppearance> Results =
            new ConditionalWeakTable<MinionStartingStats, RandomizedAppearance>();

        internal static void Randomize(MinionStartingStats stats)
        {
            if (stats?.personality == null || Results.TryGetValue(stats, out _))
                return;

            if (!AppearancePools.TryGet(stats.personality.model, out List<Personality> pool))
            {
                Debug.LogWarning($"[RandomDupes] No appearance donors available for model '{stats.personality.model}'. Keeping the original appearance.");
                return;
            }

            Personality nameDonor = Pick(pool);
            Personality hairDonor = Pick(pool);
            Personality faceDonor = Pick(pool);
            Personality bodyDonor = Pick(pool);
            Personality skinDonor = Pick(pool);

            KCompBuilder.BodyData original = MinionStartingStats.CreateBodyData(stats.personality);
            KCompBuilder.BodyData hair = MinionStartingStats.CreateBodyData(hairDonor);
            KCompBuilder.BodyData face = MinionStartingStats.CreateBodyData(faceDonor);
            KCompBuilder.BodyData body = MinionStartingStats.CreateBodyData(bodyDonor);
            KCompBuilder.BodyData skin = MinionStartingStats.CreateBodyData(skinDonor);
            KCompBuilder.BodyData mixed = original;

            AccessorySlots slots = Db.Get().AccessorySlots;

            if (IsValidHair(slots, hair.hair))
                mixed.hair = hair.hair;

            if (slots.Eyes.Lookup(face.eyes) != null)
                mixed.eyes = face.eyes;
            if (slots.Mouth.Lookup(face.mouth) != null)
                mixed.mouth = face.mouth;

            ApplyBodyIfValid(slots, body, ref mixed);
            ApplySkinIfValid(slots, skin, ref mixed);

            stats.Name = nameDonor.Name;
            Results.Add(stats, new RandomizedAppearance(
                mixed,
                nameDonor.Id,
                hairDonor.Id,
                faceDonor.Id,
                bodyDonor.Id,
                skinDonor.Id));
        }

        internal static bool TryGet(MinionStartingStats stats, out RandomizedAppearance appearance)
        {
            appearance = null;
            return stats != null && Results.TryGetValue(stats, out appearance);
        }

        private static Personality Pick(List<Personality> pool)
        {
            return pool[UnityEngine.Random.Range(0, pool.Count)];
        }

        private static bool IsValidHair(AccessorySlots slots, HashedString hair)
        {
            if (slots.Hair.Lookup(hair) == null)
                return false;

            string hairId = HashCache.Get().Get(hair);
            return !string.IsNullOrEmpty(hairId) && slots.HatHair.Lookup("hat_" + hairId) != null;
        }

        private static void ApplyBodyIfValid(
            AccessorySlots slots,
            KCompBuilder.BodyData candidate,
            ref KCompBuilder.BodyData result)
        {
            if (slots.Body.Lookup(candidate.body) == null ||
                slots.Arm.Lookup(candidate.arms) == null ||
                slots.ArmLower.Lookup(candidate.armslower) == null)
                return;

            result.body = candidate.body;
            result.arms = candidate.arms;
            result.armslower = candidate.armslower;

            if (slots.Neck.Lookup(candidate.neck) != null)
                result.neck = candidate.neck;
            if (slots.Pelvis.Lookup(candidate.pelvis) != null)
                result.pelvis = candidate.pelvis;
            if (slots.Leg.Lookup(candidate.legs) != null)
                result.legs = candidate.legs;
            if (slots.Foot.Lookup(candidate.foot) != null)
                result.foot = candidate.foot;
            if (slots.Hand.Lookup(candidate.hand) != null)
                result.hand = candidate.hand;
            if (slots.Cuff.Lookup(candidate.cuff) != null)
                result.cuff = candidate.cuff;
            if (slots.Belt.Lookup(candidate.belt) != null)
                result.belt = candidate.belt;
        }

        private static void ApplySkinIfValid(
            AccessorySlots slots,
            KCompBuilder.BodyData candidate,
            ref KCompBuilder.BodyData result)
        {
            if (slots.HeadShape.Lookup(candidate.headShape) == null ||
                slots.ArmLowerSkin.Lookup(candidate.armLowerSkin) == null ||
                slots.ArmUpperSkin.Lookup(candidate.armUpperSkin) == null ||
                slots.LegSkin.Lookup(candidate.legSkin) == null)
                return;

            result.headShape = candidate.headShape;
            result.armLowerSkin = candidate.armLowerSkin;
            result.armUpperSkin = candidate.armUpperSkin;
            result.legSkin = candidate.legSkin;
        }

    }
}
