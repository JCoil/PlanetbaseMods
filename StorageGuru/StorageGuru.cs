using Planetbase;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Redirection;
using ModWrapper;

namespace StorageGuru
{
    public class StorageGuruMod : ModBase, IMod
    { 
        public static StorageManifestController StorageController { get; set; }

        public static GameStateGame GameAccess { get; private set; }
        public static List<ResourceType> MasterResourceDefinitions { get; private set; }

        public override void Init()
        {
            ContentManager.Init();
            Redirector.PerformRedirections();

            Debug.Log("[MOD] Storage Guru activated");
        }

        public override void Update(float timeStep)
        {
            //Debug.Log("[MOD] Storage Guru updating");
            StorageGuruMod.GameAccess = Game;

            MenuController.Update(Game.mMenuSystem);
        }

        public override void OnGameStart()
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
        private Dictionary<Character, Module> newCharacterTargets;

        private Dictionary<Character, Module> characterTargets = new Dictionary<Character, Module>();

        [Obsolete("Storage targeting now handled by a redirect to StorageModuleRedirect from Module.findStorage")]
        private void RedirectCharacters()
        {
            carriedResources = Character.mCharacters.Where(x => x.getLoadedResource() != null)
                .ToDictionary(y => y, x => x.getLoadedResource()); // Get all carried resources across all characters

            newCarriedResources = carriedResources.Where(x => !characterTargets.ContainsKey(x.Key))
                .ToDictionary(y => y.Key, x => x.Value); // That haven't been processed here yet

            if (newCarriedResources.Count == 0) { return; }

            newCarriedResources = newCarriedResources.Where(x => // That are bound for a storage module
            x.Key.getTarget() is Target target
            && target.getSelectable() is Module module
            && module.isBuilt()
            && module.getCategory() == Module.Category.Storage)
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
                    kvp.Key.setTarget(new Target(kvp.Value, kvp.Value.getRadius() / 1.8f));
                }

                characterTargets.Add(kvp.Key, kvp.Value);
            }
        }

        public Module FindNearestModule(Vector3 position, List<Module> modules)
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

        #endregion
    }
}
