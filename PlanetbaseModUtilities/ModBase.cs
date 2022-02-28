using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;
using static UnityModManagerNet.UnityModManager;
using UnityEngine;

namespace PlanetbaseModUtilities
{
    /// <summary>
    /// Common base class for Mod entry points
    /// </summary>
    public abstract class ModBase
    {
        /// <summary>
        /// Mods inheriting from ModBase must implement this and call InitializeMod() to correctly register themelves with UMM and do any Harmony setup.
        /// The Info.json file should note ModName.Init as its entry point
        /// See SampleMod.cs for full example
        /// </summary>
        /// <param name="entry"></param>
        public static void Init(ModEntry entry) { }

        public static GameStateGame GetGameStateGame() => GameManager.getInstance().getGameState() as GameStateGame;

        #region Class Contract

        /// <summary>
        /// Called once mod is initialised + registered with UnityModManager
        /// </summary>
        public abstract void OnInitialized(ModEntry modEntry);

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

        public static ModBase Instance { get; private set; } 

        public static void InitializeMod(ModBase mod, ModEntry modEntry, string modName)
        {
            Instance = mod;

            modEntry.OnUpdate = mod.OnUpdate;
            GameManagerPatch.OnGameStateChanged = mod.OnGameStateChanged;

            mod.OnInitialized(modEntry);

            var harmony = new Harmony(modName);
            harmony.PatchAll();
        }

        #endregion

        protected virtual void LoadConfiguration()
        {

        }
    }
}
