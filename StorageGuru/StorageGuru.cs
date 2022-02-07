using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using System.Reflection;
using PlanetbaseModUtilities;
using System.IO;
using System.Linq;

namespace StorageGuru
{
    public class StorageGuruMod : ModBase
    { 
        public static StorageManifestController StorageController { get; set; }

        public static GameStateGame GameAccess { get; private set; }
        FieldInfo gameAccess = typeof(GameStateGame).GetField("Mode", BindingFlags.NonPublic | BindingFlags.Static);
        public static List<ResourceType> MasterResourceDefinitions { get; private set; }

        public static void Init(ModEntry modEntry)
        {
            InitializeMod(new StorageGuruMod(), modEntry, "StorageGuru");
            ContentManager.Init();

            Debug.Log("[MOD] Storage Guru activated");
        }

        public void Update(float timeStep)
        {
            //Debug.Log("[MOD] Storage Guru updating");
            gameAccess = Game;

            MenuController.Update(Game.mMenuSystem);
        }

        public void OnGameStart()
        {
            MenuController.Init(Game.mMenuSystem);
            RefreshResourceDefinitions();

            if (StorageController == null)
            {
                StorageController = new StorageManifestController();
            }

            StorageController.ConsolidateManifest();
            ContentManager.CreateAlternativeIcons(MasterResourceDefinitions);
        }

        private void RefreshResourceDefinitions()
        {
            MasterResourceDefinitions = TypeList<ResourceType, ResourceTypeList>.getInstance().mTypeList
                .Where(x => !(x is Coins)).ToList();  // Clear out non-storeable resources (coins)
        }

        public static void Log(string message)
        {
            Debug.Log($"[Mod] Storage Guru: {message}");
        }

        #region Character Targeting

        private Dictionary<Character, Resource> carriedResources;
        private Dictionary<Character, Resource> newCarriedResources;
        private Dictionary<Character, Planetbase.Module> newCharacterTargets;

        private Dictionary<Character, Planetbase.Module> characterTargets = new Dictionary<Character, Planetbase.Module>();

        [Obsolete("Storage targeting now handled by a redirect to StorageModuleRedirect from Module.findStorage")]
#pragma warning disable IDE0051 // Remove unused private members
        private void RedirectCharacters()
#pragma warning restore IDE0051 // Remove unused private members
        {
            FieldInfo characters = typeof(Character).GetField("mCharacters", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.IgnoreReturn);
            var value = characters.GetValue(characters) as List<Character>;
            carriedResources = value.Where(x => x.getLoadedResource() != null)
                .ToDictionary(y => y, x => x.getLoadedResource()); // Get all carried resources across all characters

            newCarriedResources = carriedResources.Where(x => !characterTargets.ContainsKey(x.Key))
                .ToDictionary(y => y.Key, x => x.Value); // That haven't been processed here yet

            if (newCarriedResources.Count == 0) { return; }

            MethodInfo cat = typeof(Planetbase.Module).GetMethod("getCategory()", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.IgnoreReturn);
            var value2 = cat.GetMethodBody();
            newCarriedResources = newCarriedResources.Where(x => // That are bound for a storage module
            x.Key.getTarget() is Target target
            && target.getSelectable() is Planetbase.Module module
            && module.isBuilt()
            && value2 == Planetbase.Module.Category.Storage)
                .ToDictionary(x => x.Key, y => y.Value);

            // Index resources and list valid target modules for each
            newCharacterTargets = newCarriedResources.ToDictionary(
                x => x.Key,
                y => FindNearestModule(y.Key.getPosition(), StorageController.ListValidModules(y.Value.getResourceType())));

            characterTargets = characterTargets.Where(x => newCarriedResources.ContainsKey(x.Key)).ToDictionary(x => x.Key, y => y.Value); // Removed completed characters from list

            foreach (var kvp in newCharacterTargets)
            {
                if (kvp.Value != null)
                {
                    MethodInfo target = typeof(Character).GetMethod("setTarget()", BindingFlags.NonPublic | BindingFlags.Instance);
                    var value3 = target.GetMethodBody();
                    value3(new Target(kvp.Value, kvp.Value.getRadius() / 1.8f));
                }

                characterTargets.Add(kvp.Key, kvp.Value);
            }
        }

        public Planetbase.Module FindNearestModule(Vector3 position, List<Planetbase.Module> modules)
        {
            try
            {
                if (modules != null && modules.Count > 0 && position != null)
                {
                    var orderedModules = modules.OrderBy(x => x.distanceTo(position)).ToList();

                    if (orderedModules != null && orderedModules.Count > 0)
                    {
                        return orderedModules[0];
                    }
                }
            }
            catch (NullReferenceException)
            {
            }

            return null;
        }

		public override void OnInitialized()
		{
			throw new NotImplementedException();
		}

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
