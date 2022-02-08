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
    public abstract class ModBase
    {
        public GameStateGame Game { get; private set; }
        public GuiMenuSystem MenuSystem { get; private set; }

        #region Class Contract

        /// <summary>
        /// Called once the initialization factory finishes
        /// </summary>
        public abstract void OnInitialized();

        /// <summary>
        /// Called every timestep while game is running
        /// </summary>
        public abstract void OnUpdate(ModEntry modEntry, float timeStep); 

        private void Update(ModEntry modEntry, float timeStep)
        {
            if (GameManager.getInstance().getGameState() is GameStateGame game)
            {
                Game = game;
            }

            OnUpdate(modEntry, timeStep);
        }

        #endregion

        #region Init Factory

        public static void Init(ModEntry entry) { }

        public static ModBase Instance { get; private set; } 

        public static Harmony Harmony { get; private set; }

        public static void InitializeMod(ModBase mod, ModEntry modEntry, string modName)
        {
            Instance = mod;

            modEntry.OnUpdate = mod.OnUpdate;

            Harmony = new Harmony(modName);
            try
            {
                Harmony.PatchAll();
            }
            catch (HarmonyException e)
            {
                modEntry.Logger.Error(e.Message);
            }

            mod.OnInitialized();
        }

        #endregion
    }
}
