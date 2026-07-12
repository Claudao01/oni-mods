using System.IO;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using UnityEngine;

namespace CLD01_MeteorDigOrders
{
    public sealed class MeteorDigOrdersMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            PUtil.InitLibrary();
            LocString.CreateLocStringKeys(typeof(STRINGS), "CLD01_MeteorDigOrders.");
            new POptions().RegisterOptions(this, typeof(MeteorDigOrdersOptions));
            new PLocalization().Register();
            EnsureDefaultConfig();

            Debug.Log("[CLD01_MeteorDigOrders] Meteor Dig Orders 0.1.0 loaded.");
        }

        private static void EnsureDefaultConfig()
        {
            string path = POptions.GetConfigFilePath(typeof(MeteorDigOrdersOptions));
            if (!File.Exists(path))
                POptions.WriteSettings(new MeteorDigOrdersOptions());
        }
    }
}
