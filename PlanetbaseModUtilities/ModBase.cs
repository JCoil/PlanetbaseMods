using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnityModManagerNet.UnityModManager;

namespace PlanetbaseModUtilities
{
    public abstract class ModBase
    {
        #region Class Contract

        /// <summary>
        /// Called once the initialization factory finishes
        /// </summary>
        public abstract void OnInitialized();

        /// <summary>
        /// Called every timestep while game is running
        /// </summary>
        public abstract void OnUpdate(ModEntry modEntry, float timeStep);

        /// <summary>
        /// Called when we enter GameStateGame
        /// </summary>
        //public abstract void OnGameStart();

        #endregion

        #region Init Factory

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
