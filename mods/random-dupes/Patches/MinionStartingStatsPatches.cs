using System;
using System.Collections.Generic;
using Database;
using HarmonyLib;
using RandomDupes.Randomization;
using UnityEngine;

namespace RandomDupes.Patches
{
    internal static class MinionStartingStatsPatchShared
    {
        internal static void AfterConstruction(MinionStartingStats instance)
        {
            DupeAppearanceRandomizer.Randomize(instance);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), MethodType.Constructor, new Type[]
    {
        typeof(Personality), typeof(string), typeof(string), typeof(bool)
    })]
    internal static class PersonalityConstructorPatch
    {
        private static void Postfix(MinionStartingStats __instance)
        {
            MinionStartingStatsPatchShared.AfterConstruction(__instance);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), MethodType.Constructor, new Type[]
    {
        typeof(bool), typeof(string), typeof(string), typeof(bool)
    })]
    internal static class RandomConstructorPatch
    {
        private static void Postfix(MinionStartingStats __instance)
        {
            MinionStartingStatsPatchShared.AfterConstruction(__instance);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), MethodType.Constructor, new Type[]
    {
        typeof(Tag), typeof(bool), typeof(string), typeof(string), typeof(bool)
    })]
    internal static class ModelConstructorPatch
    {
        private static void Postfix(MinionStartingStats __instance)
        {
            MinionStartingStatsPatchShared.AfterConstruction(__instance);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), MethodType.Constructor, new Type[]
    {
        typeof(List<Tag>), typeof(bool), typeof(string), typeof(string), typeof(bool)
    })]
    internal static class ModelsConstructorPatch
    {
        private static void Postfix(MinionStartingStats __instance)
        {
            MinionStartingStatsPatchShared.AfterConstruction(__instance);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.ApplyAccessories))]
    internal static class ApplyAccessoriesPatch
    {
        private static bool Prefix(MinionStartingStats __instance, GameObject go)
        {
            if (!DupeAppearanceRandomizer.TryGet(__instance, out RandomizedAppearance appearance))
                return true;

            if (go == null)
                return false;

            Accessorizer accessorizer = go.GetComponent<Accessorizer>();
            if (accessorizer == null)
            {
                Debug.LogWarning("[RandomDupes] Target has no Accessorizer; the original appearance method will run.");
                return true;
            }

            accessorizer.ApplyBodyData(appearance.BodyData);
            accessorizer.UpdateHairBasedOnHat();
            return false;
        }
    }
}
