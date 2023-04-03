using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using System.Reflection;
using PlanetbaseModUtilities;
using PlanetbaseModUtilities.Utils;
using BuildingAligner.Patches;

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
            DebugRendererUtils.AddGroup("Connections");
            Debug.Log("[MOD] BuildingAligner activated");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if (GetGameStateGame() is GameStateGame gameStateGame && !gameStateGame.IsMode(GameStateUtils.Mode.PlacingModule))
            {
                TryPlaceModulePatch.IsRendering = false;
                DebugRendererUtils.ClearGroup("Connections");
            }
        }
    }
}
