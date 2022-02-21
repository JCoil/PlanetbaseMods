using System.Collections.Generic;
using System.Linq;
using Planetbase;
using UnityEngine;
using System.IO;
using System;
using System.Xml;
using PlanetbaseModUtilities;

namespace StorageGuru
{
    public class StorageManifestController
    { 
        private List<ManifestEntry> StoreManifest;
        private List<ManifestEntry.Blueprint> Blueprints;

        public StorageManifestController()
        {
            StoreManifest = new List<ManifestEntry>();
            Blueprints = new List<ManifestEntry.Blueprint>();
        }

        public void Update()
        {
            if ((GetAllActualStorageModules() ?? new List<Module>()).Count != StoreManifest.Count)
            {
                ConsolidateManifest();
            }
        }

        /// <summary>
        /// Check for new or remove storage modules and update manifest
        /// </summary>
        public void ConsolidateManifest()
        {
            var allActualStorage = GetAllActualStorageModules() ?? new List<Module>();
            var allManifestStorage = GetAllManifestModules();

            if(allActualStorage.Count > allManifestStorage.Count)
            {
                // New storage module added
                // Find all new modules not in storeManifest
                var addedModules = allActualStorage.Except(allManifestStorage).ToList();

                foreach (var module in addedModules)
                {
                    AddManifestEntry(module, null);
                }
            }
            else if(allActualStorage.Count < allManifestStorage.Count)
            {
                // Storage modules removed
                // Find all manifest modules not in allStorage
                var removedModules = allManifestStorage.Except(allActualStorage).ToList();

                foreach (var module in removedModules)
                {
                    RemoveManifestEntry(module);
                }
            }
        }

        #region File IO

        public void AddBlueprint(ManifestEntry.Blueprint blueprint)
        {
            if (blueprint != null)
            {
                Blueprints.Add(blueprint);
            }
        } 

        #endregion

        #region Manifest Management

        private void RefreshStorageModule(Module module)
        {
            if (module == null || module.GetResourceStorageObject() == null || module.GetResourceStorageObject().GetSlots() == null)
            {
                return;
            }

            if (GetManifestEntry(module) is ManifestEntry entry)
            {
                foreach (var slot in module.GetResourceStorageObject().GetSlots())
                {
                    if (slot != null && slot.GetResources() != null)
                    {
                        foreach (var resource in slot.GetResources())
                        {
                            if (entry.ContainsResource(resource.getResourceType()))
                            {
                                // If the resource is now allowed to be stored here after manifest change
                                resource.setState(Resource.State.Stored);
                            }
                            else
                            {
                                // If the resource should be marked to be picked up
                                resource.setState(Resource.State.Idle);
                            }
                        }
                    }
                }
            }
        }

        public void ToggleDefinitions(Module module, ResourceType resource)
        {
            if(GetManifestEntry(module) is ManifestEntry entry)
            {
                if(entry.ContainsResource(resource))
                {
                    entry.RemoveResourceDefinitions(resource);
                }
                else
                {
                    entry.AddResourceDefinitions(resource);
                }

                DefinitionChanged(module);
            }
        }

        private void DefinitionChanged(Module module)
        {
            RefreshStorageModule(module);
            //SaveManifest();
        }

        public List<Module> GetAllManifestModules()
        {
            return StoreManifest.Select(x => x.StorageModule).ToList();
        }

        public static List<Module> GetAllActualStorageModules()
        {
            return Module.getCategoryModules(Module.Category.Storage)?.Where(x => x != null).ToList();
        }

        public List<Module> ListValidModules(ResourceType resourceType)
        {
            var validModules = StoreManifest.Where(x =>
                x.ContainsResource(resourceType)
                && x.StorageModule.getEmptyStorageSlotCount() > x.StorageModule.getPotentialUserCount(null)).ToList(); 

            return validModules.Select(x => x.StorageModule).ToList();
        }

        public Module GetModuleById(int id)
        {
            return GetAllActualStorageModules()?.FirstOrDefault(x => x.getId() == id);
        }

        public ResourceType GetResourceTypeByName(string name)
        {
            var resource = StorageGuruMod.MasterResourceDefinitions.FirstOrDefault(x => x.getName() == name);

            if(resource==null)
            {
                Debug.Log($"[STORAGEGURU] Couldn't parse resource '{name}'");
            }

            return resource;
        }

        #endregion

        #region Manifest Entries

        public void AddManifestEntry(Module module, List<ResourceType> resourceTypes)
        {
            if (GetManifestEntry(module) == null)
            {
                if(resourceTypes == null)
                {
                    resourceTypes = new List<ResourceType>();
                    resourceTypes.AddRange(StorageGuruMod.MasterResourceDefinitions);
                }

                StoreManifest.Add(new ManifestEntry(module, resourceTypes));
            }
        }

        public ManifestEntry GetManifestEntry(Module module)
        {
            return StoreManifest.FirstOrDefault(x => x.StorageModule == module);
        }

        private void RemoveManifestEntry(Module module)
        {
            if (GetManifestEntry(module) is ManifestEntry entry)
            {
                StoreManifest.Remove(entry);
            }
        }

        public void AddAllDefinitionsToManifestEntry(Module module)
        {
            if (GetManifestEntry(module) is ManifestEntry entry)
            {
                entry.AddResourceDefinitions(StorageGuruMod.MasterResourceDefinitions.ToArray());
            }

            DefinitionChanged(module);
        }

        public void RemoveAllDefinitionsFromManifestEntry(Module module)
        {
            if (GetManifestEntry(module) is ManifestEntry entry)
            {
                entry.Clear();
            }

            DefinitionChanged(module);
        }

        public bool HasDefinition(Module module, ResourceType resourceType)
        {
            if (GetManifestEntry(module) is ManifestEntry entry)
            {
                return entry.ContainsResource(resourceType);
            }

            return false;
        }

        public bool HasDefinition(Module module, Resource resource)
        {
            return HasDefinition(module, resource.getResourceType());
        }

        #endregion
    }    
}
