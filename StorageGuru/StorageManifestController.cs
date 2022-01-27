using System.Collections.Generic;
using System.Linq;
using Planetbase;
using UnityEngine;
using System.IO;
using System;
using System.Xml;

namespace StorageGuru
{
    public class StorageManifestController
    {
        // Legacy storage - to be removed
        readonly string ManifestDirectoryPath = @"Mods\StorageGuruData\Manifests\";
        readonly string ManifestAutoFileName = @"autosave.txt";
        private string ManifestFileName;

        private bool Loading;

        private List<ManifestEntry> StoreManifest;
        private List<ManifestEntry.Blueprint> Blueprints;

        public StorageManifestController()
        {
            StoreManifest = new List<ManifestEntry>();
            Loading = true;

            ManifestDirectoryPath = Path.Combine(Util.getFilesFolder(), ManifestDirectoryPath);
            ManifestFileName = $"manifest_{Path.GetFileNameWithoutExtension(SaveData.getSavePath())}.txt";

            if (LoadManifest(ManifestDirectoryPath + ManifestFileName))
            {
                Debug.Log("Loaded from manifest");
            }
            else if (LoadManifest(ManifestDirectoryPath + ManifestAutoFileName))
            {
                Debug.Log("Loaded from autosave manifest");
            }

            ConsolidateManifest();
            Loading = false;
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
            if (Blueprints != null)
            {
                foreach (var blueprint in Blueprints)
                {
                    LoadEntryFromBlueprint(blueprint);
                }

                Blueprints = null;
            }

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

        const char SerializeModuleSeperator = ',';

        [Obsolete("No longer required - saving handled by Game's xml serialization")]
        private bool LoadManifest(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var contents = File.ReadAllText(path);

                    if (contents.Split(SerializeModuleSeperator) is string[] splitEntries)
                    {
                        foreach (var s in splitEntries)
                        {
                            LoadEntryFromBlueprint(ManifestEntry.Blueprint.Deserialize(s));
                        }
                    }

                    return true;
                }
                catch (Exception)
                {
                    Debug.Log($"[StorageGuru] Failed to deserialize manifest {path}");
                }
            }

            return false;
        }

        public void Serialize(XmlNode parent, string name)
        {
            XmlNode parent2 = Serialization.createNode(parent, name, null);

            foreach (var entry in StoreManifest)
            {
                XmlNode parent3 = Serialization.createNode(parent2, "storage-module");

                Serialization.serializeInt(parent3, "module-id", entry.ModuleId);
                Serialization.serializeList(parent3, "resource-types", entry.AllowedResources.Select(x => x.GetType().Name).ToList());
            }
        }

        public void Deserialize(XmlNode parent)
        {
            Blueprints = new List<ManifestEntry.Blueprint>();

            foreach (var obj in parent.ChildNodes)
            {
                if (obj is XmlNode xmlNode && xmlNode.Name == "storage-module")
                {
                    var moduleId = Serialization.deserializeInt(xmlNode["module-id"]);
                    var resources = Serialization.deserializeList<string>(xmlNode["resource-types"]);

                    Blueprints.Add(new ManifestEntry.Blueprint(moduleId, resources));
                }
            }
        }

        private void LoadEntryFromBlueprint(ManifestEntry.Blueprint blueprint)
        {
            if (GetModuleById(blueprint.ModuleId) is Module module)
            {
                var resourceTypes = new List<ResourceType>();

                if (blueprint.Resources != null)
                {
                    foreach (string resourceName in blueprint.Resources)
                    {
                        if (TypeList<ResourceType, ResourceTypeList>.find(resourceName) is ResourceType resourceType)
                        {
                            resourceTypes.Add(resourceType);
                        }
                    }
                }

                AddManifestEntry(module, resourceTypes);
            }
        }

        #endregion

        #region Manifest Management

        private void RefreshStorageModule(Module module)
        {
            if (module == null || module.mResourceStorage == null || module.mResourceStorage.mSlots == null)
            {
                return;
            }

            if (GetManifestEntry(module) is ManifestEntry entry)
            {
                foreach (var slot in module.mResourceStorage.mSlots)
                {
                    if (slot != null && slot.mResources != null)
                    {
                        foreach (var resource in slot.mResources)
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

        private void AddManifestEntry(Module module, List<ResourceType> resourceTypes)
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

        public bool HasDefinition(Module module, ResourceType resource)
        {
            if (GetManifestEntry(module) is ManifestEntry entry)
            {
                return entry.ContainsResource(resource);
            }

            return false;
        }

        #endregion
    }    
}
