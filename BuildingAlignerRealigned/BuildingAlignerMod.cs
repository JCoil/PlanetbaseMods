using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using System.Reflection;
using PlanetbaseModUtilities;

namespace BuildingAligner
{
    public class BuildingAlignerMod : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new BuildingAlignerMod(), modEntry, "BuildingAligner Realigned");

        public override void OnInitialized(ModEntry modEntry)
        {
            //StringUtils.RegisterString("cheat_menu", "Cheat Tools");
        }

        public void Init()
        {
            //DebugRenderer.addGroup("Connections");
            //Redirector.PerformRedirections();
            //Debug.Log("[MOD] BuildingAligner activated");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //GameStateGame gameState = GameManager.getInstance().getGameState() as GameStateGame;
            //if (gameState != null && gameState.mMode != GameStateGame.Mode.PlacingModule)
            //{
            //    rendering = false;
            //    //DebugRenderer.clearGroup("Connections");
            //}
        }
    }
}
