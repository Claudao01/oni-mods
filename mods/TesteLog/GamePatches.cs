using HarmonyLib;
using UnityEngine;

namespace TesteLog
{
    // Aplica um Postfix em Game.OnPrefabInit, executado logo no inicio
    // da inicializacao da partida.
    // OnPrefabInit e protected em Game, por isso e referenciado por string.
    [HarmonyPatch(typeof(Game), "OnPrefabInit")]
    public static class Game_OnPrefabInit_Patch
    {
        public static void Postfix()
        {
            Debug.Log("Meu primeiro mod ONI carregou!");
        }
    }
}
