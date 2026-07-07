using HarmonyLib;
using KMod;

namespace TesteLog
{
    // Ponto de entrada do mod. O KMod instancia esta classe e, ao chamar
    // base.OnLoad(harmony), o Harmony aplica automaticamente todos os
    // patches ([HarmonyPatch]) declarados neste assembly (PatchAll).
    public class TesteLogMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
        }
    }
}
