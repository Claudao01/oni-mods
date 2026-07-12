using HarmonyLib;
using KMod;
using UnityEngine;

namespace RandomDupes
{
    public sealed class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            Debug.Log("[RandomDupes] Random Dupes 0.1.0 loaded.");
        }
    }
}
