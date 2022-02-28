using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace PlanetbaseModUtilities
{
#if FALSE

    /// <summary>
    /// Sample class to show how to inherit from ModBase and initialize a custom mod. 
    /// Leave above FALSE to skip compiling
    /// </summary>
    public class SampleMod : ModBase
    {
        /// <summary>
        /// This is required for UMM to load the mod
        /// The Info.json file should note ModName.Init as its entry point
        /// InitializeMod() handles the common mod creation work
        /// </summary
        public static new void Init(ModEntry modEntry) => InitializeMod(new SampleMod(), modEntry, "SampleMod");
                
        public override void OnInitialized()
        {
            // Do initial set up of mod
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            // Do set up when a game is loaded
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Do work on every update
        }
    }
#endif
}
