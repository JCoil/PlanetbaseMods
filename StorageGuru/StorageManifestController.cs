using System.Collections.Generic;
using System.Linq;
using Planetbase;
using UnityEngine;
using System.IO;
using System;

namespace StorageGuru
{
    public class StorageManifestController
    {
        readonly string LegacyManifestFilePath = @"Mods\Settings\storage_manifest.txt";
        readonly string ManifestDirectoryPath = @"Mods\StorageGuru\Manifests\";
        readonly string ManifestAutoFileName = @"autosave.txt";
        private string ManifestFileName;

        private List<ManifestEntry> StoreManifest = new List<ManifestEntry>(); 

        private bool Loading;

        public StorageManifestController()
        {
            Loading = true;

            LegacyManifestFilePath = Path.Combine(Util.getFilesFolder(), LegacyManifestFilePath);
            ManifestDirectoryPath = Path.Combine(Util.getFilesFolder(), ManifestDirectoryPath);
            ManifestFileName = $"manifest_{Path.GetFileNameWithoutExtension(SaveData.getSavePath())}.txt";

            if (LoadManifest(ManifestDirectoryPath + ManifestFileName))
            {
                Debug.Log("Loaded from manifest");
            }
            else if(LoadManifest(ManifestDirectoryPath + ManifestAutoFileName))
            {
                Debug.Log("Loaded from autosave manifest");
            }
            else if (LoadManifestLegacy(LegacyManifestFilePath))
            {
                Debug.Log("Loaded from legacy manifest");
                File.Delete(LegacyManifestFilePath);
            }

            ConsolidateManifest();
            Loading = false;
            SaveManifest();
        }

        /// <summary>
        /// Check for new or remove storage modules and update manifest
        /// </summary>
        public void ConsolidateManifest()
        {
            var allActualStorage = GetAllActualStorageModules();
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

        private void SaveManifest()
        {
            ManifestFileName = $"manifest_{Path.GetFileNameWithoutExtension(SaveData.getSavePath())}.txt";

            if (!Loading)
            {
                var contents = string.Join(
                    SerializeModuleSeperator + Environment.NewLine,
                    StoreManifest.Select(x => x.Serialize()).ToArray());

                Directory.CreateDirectory(ManifestDirectoryPath);

                File.WriteAllText(ManifestDirectoryPath + ManifestFileName, contents);
                File.WriteAllText(ManifestDirectoryPath + ManifestAutoFileName, contents);
            }
        }

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
                            LoadEntryFromBlueprint(ManifestEntry.ManifestEntryBlueprint.Deserialize(s));
                        }
                    }

                    return true;
                }
                catch(Exception)
                {

                }
            }

            return false;
        }

        private void LoadEntryFromBlueprint(ManifestEntry.ManifestEntryBlueprint blueprint)
        {
            if (GetModuleById(blueprint.ModuleId) is Module module)
            {
                var resourceTypes = new List<ResourceType>();
                var primaryResourceTypes = new List<ResourceType>();

                if (blueprint.Resources != null)
                {
                    foreach (string resourceName in blueprint.Resources)
                    {
                        var isPrimary = resourceName.Contains("!");

                        if (GetResourceTypeByName(resourceName.Replace("!", string.Empty)) is ResourceType resourceType)
                        {
                            resourceTypes.Add(resourceType);

                            if(isPrimary)
                            {
                                primaryResourceTypes.Add(resourceType);
                            }
                        }
                    }
                }

                AddManifestEntry(module, resourceTypes);
            }
        }

        public bool LoadManifestLegacy(string filePath)
        {
            var serializer = new SerializeHelper();
            Dictionary<int, List<string>> loadedDefinitions = new Dictionary<int, List<string>>();

            if (File.Exists(filePath))
            {
                try
                {
                    string[] contents = File.ReadAllLines(filePath);
                    loadedDefinitions = serializer.DeserializeManifest(contents); 

                    foreach (var definition in loadedDefinitions)
                    {
                        Module module = GetModuleById(definition.Key);

                        if (module != null)
                        {
                            AddManifestEntry(module, null);
                            var entry = GetManifestEntry(module);

                            if (entry != null && definition.Value != null && definition.Value.Count > 0)
                            {
                                entry.AddResourceDefinitions(definition.Value.Select(x => GetResourceTypeByName(x)).ToArray());
                            }
                        }
                    }

                    return true;
                }
                catch (Exception)
                {
                    Debug.Log("Failed to load legacy manifest");
                }
            }

            return false;
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
                                resource.setState(Resource.State.Stored);
                            }
                            else
                            {
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
            SaveManifest();
        }

        public List<Module> GetAllManifestModules()
        {
            return StoreManifest.Select(x => x.StorageModule).ToList();
        }

        public static List<Module> GetAllActualStorageModules()
        {
            return Module.getCategoryModules(Module.Category.Storage).Where(x => x != null).ToList();
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
            var storage = GetAllActualStorageModules();

            return storage == null ? null : storage.FirstOrDefault(x => x.getId() == id);
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
