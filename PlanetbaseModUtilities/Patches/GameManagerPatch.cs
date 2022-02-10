using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlanetbaseModUtilities
{
    [HarmonyPatch(typeof(GameManager), "onNewGameState")]
    public class GameManagerPatch
    {
        public delegate void GameStateChangedDelegate(GameState gameState);
        public static GameStateChangedDelegate OnGameStateChanged;

        /// <summary>
        /// Add hook at end of onNewGameState to listen for state changes
        /// </summary>
        public static void Postfix(GameState gameState)
        {
            OnGameStateChanged?.Invoke(gameState);
        }
    }
}
