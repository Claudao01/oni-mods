using System;
using System.Collections.Generic;
using Database;
using HarmonyLib;
using RandomDupes.Diagnostics;
using RandomDupes.Randomization;
using UnityEngine;

namespace RandomDupes.Patches
{
    internal static class MinionStartingStatsPatchShared
    {
        internal static void AfterConstruction(MinionStartingStats instance, bool starter)
        {
            DupeAppearanceRandomizer.Randomize(instance, starter);
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
            MinionStartingStatsPatchShared.AfterConstruction(__instance, false);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), MethodType.Constructor, new Type[]
    {
        typeof(bool), typeof(string), typeof(string), typeof(bool)
    })]
    internal static class RandomConstructorPatch
    {
        private static void Postfix(MinionStartingStats __instance, bool is_starter_minion)
        {
            MinionStartingStatsPatchShared.AfterConstruction(__instance, is_starter_minion);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), MethodType.Constructor, new Type[]
    {
        typeof(Tag), typeof(bool), typeof(string), typeof(string), typeof(bool)
    })]
    internal static class ModelConstructorPatch
    {
        private static void Postfix(MinionStartingStats __instance, bool is_starter_minion)
        {
            MinionStartingStatsPatchShared.AfterConstruction(__instance, is_starter_minion);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), MethodType.Constructor, new Type[]
    {
        typeof(List<Tag>), typeof(bool), typeof(string), typeof(string), typeof(bool)
    })]
    internal static class ModelsConstructorPatch
    {
        private static void Postfix(MinionStartingStats __instance, bool is_starter_minion)
        {
            MinionStartingStatsPatchShared.AfterConstruction(__instance, is_starter_minion);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.ApplyAccessories))]
    internal static class ApplyAccessoriesPatch
    {
        private static bool Prefix(MinionStartingStats __instance, GameObject go)
        {
            try
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
            catch (Exception exception)
            {
                ErrorLogger.Write("Applying randomized physical appearance", exception);
                return true;
            }
        }
    }
}
