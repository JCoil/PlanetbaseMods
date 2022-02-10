using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;
using static UnityModManagerNet.UnityModManager;
using UnityEngine;
using PlanetbaseModUtilities;

namespace PlanetbaseModUtilities
{
    public abstract class ModBase
    {
        #region Class Contract

        /// <summary>
        /// Called once mod is initialised + registered with UnityModManager
        /// </summary>
        public abstract void OnInitialized();

        /// <summary>
        /// Called every timestep while game is running
        /// </summary>
        public abstract void OnUpdate(ModEntry modEntry, float timeStep);

        /// <summary>
        /// Called when GameManager.GameState changes to GameStateGame
        /// </summary>
        public virtual void OnGameStart(GameStateGame gameStateGame) { }

        protected virtual void OnGameStateChanged(GameState gameState)
        {
            if (gameState is GameStateGame gameStateGame)
            {
                OnGameStart(gameStateGame);
            }
        }

        #endregion

        #region Init Factory

        public static void Init(ModEntry entry) { }

        public static ModBase Instance { get; private set; } 

        public static void InitializeMod(ModBase mod, ModEntry modEntry, string modName)
        {
            Instance = mod;

            modEntry.OnUpdate = mod.OnUpdate;
            GameManagerPatch.OnGameStateChanged = mod.OnGameStateChanged;

            mod.OnInitialized();

            var harmony = new Harmony(modName);
            harmony.PatchAll();
        }

        #endregion
    }
}
