using System.Collections.Generic;
using System.Linq;
using Planetbase;
using UnityEngine;
using System.IO;
using System;

namespace StorageGuru
{
    public class StorageController
    {
        private Dictionary<Module, HashSet<Type>> storeManifest;
        private SerializeHelper serializer;

        public StorageController()
        {
            serializer = new SerializeHelper();
            storeManifest = new Dictionary<Module, HashSet<Type>>();
        }

        /// <summary>
        /// Check for new or remove storage modules and update manifest
        /// </summary>
        public void ConsolidateDefinitions()
        {
            var allStorage = GetAllStorage();

            if (storeManifest != null && allStorage != null && allStorage.Count != storeManifest.Count)
            {
                UpdateManifest(allStorage);
            }
        }

        private void UpdateManifest(List<Module> allStorage)
        {
            List<Module> storageInManifest = storeManifest.Keys.ToList();

            if (allStorage.Count > storeManifest.Count)
            {
                // New storage module added
                // Find all new modules not in storeManifest
                var newModules = allStorage.Except(storageInManifest).ToList();

                //Debug.Log(newModules.Count.ToString());
                // Should only be one module
                foreach (var module in newModules)
                {
                    AddDefinitions(module, StorageGuru.ResourceDefinitions.Select(x => x.GetType()).ToArray());
                }
            }
            else if (allStorage.Count < storeManifest.Count)
            {
                // Storage module removed
                // Find all manifest modules not in allStorage
                var removedModules = storageInManifest.Except(allStorage).ToList();

                // Should only be one module
                foreach (var module in removedModules)
                {
                    RemoveDefinition(module, null);
                }
            }
        }

        public void SaveManifest(string filepath)
        {
            string contents = serializer.SerializeManifest(storeManifest);
            File.WriteAllText(filepath, contents);
        }

        public void LoadManifest(string filePath)
        {
            Dictionary<int, List<string>> loadedDefinitions = new Dictionary<int, List<string>>();

            if (File.Exists(filePath))
            {
                string[] contents = File.ReadAllLines(filePath);
                loadedDefinitions = serializer.DeserializeManifest(contents);
            }
            else
            {
                Debug.Log("[MOD] StorageGuru could not find manifest file");
            }

            foreach (var def in loadedDefinitions)
            {
                Module module = GetModuleById(def.Key);

                if (module != null)
                {
                    AddDefinition(module, null);

                    if (def.Value != null && def.Value.Count > 0)
                    {
                        foreach (string strRes in def.Value)
                        {
                            if (!String.IsNullOrEmpty(strRes))
                            {
                                Type resType = Type.GetType(strRes);
                                if (resType != null)
                                {
                                    AddDefinition(module, resType);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddDefinition(Module module, Type resource)
        {
            if(storeManifest.ContainsKey(module))
            {
                if(resource != null)
                {
                    // Add new resource filter to definition
                    storeManifest[module].Add(resource);
                }
            }
            else
            {
                if (resource == null)
                {
                    // Add new blank definition
                    storeManifest.Add(module, new HashSet<Type>());
                }
                else
                {
                    // Add new definition with filter
                    storeManifest.Add(module, new HashSet<Type> { resource });
                }
            }
        }

        private void RemoveDefinition(Module module, Type resource)
        {
            if (storeManifest.ContainsKey(module))
            {
                if (resource == null)
                {
                    // Remove entire definition
                    storeManifest.Remove(module);
                }
                else
                {
                    // Remove resource filter from definition
                    storeManifest[module].Remove(resource);
                }

                //Debug.Log("After removal count = " + GetDefinition(module).Count);
            }
        }

        public void ToggleDefinitions(Module module, params Type[] resources)
        {
            foreach (var res in resources)
            {
                if (HasDefinition(module, res))
                {
                    RemoveDefinition(module, res);
                }
                else
                {
                    AddDefinition(module, res);
                }
            }

            DefinitionChanged(module);
        }

        public void AddDefinitions(Module module, Type[] resources)
        {
            foreach (var res in resources)
            {
                AddDefinition(module, res);
            }

            DefinitionChanged(module);
        }

        public void RemoveDefinitions(Module module, Type[] resources)
        {
            foreach (var res in resources)
            {
                RemoveDefinition(module, res);
            }

            DefinitionChanged(module);
        }

        private void DefinitionChanged(Module module)
        {
            if(module != null)
            {
                StorageGuru.RefreshStorage(module, GetDefinitions(module));
                SaveManifest(StorageGuru.STORAGE_MANIFEST_PATH);
            }
        }

        public HashSet<Type> GetDefinitions(Module module)
        {
            if (storeManifest.ContainsKey(module))
            {
                return storeManifest[module];
            }
            return new HashSet<Type>();
        }

        public bool HasDefinition(Module module, Type resource)
        {
            if(module != null && resource != null)
            {
                if(storeManifest.ContainsKey(module))
                {
                    if(GetDefinitions(module).Count > 0)
                    {
                        return GetDefinitions(module).Contains(resource);
                    }
                }
            }

            return false;
        }

        public List<Module> GetValidModules(Type resource)
        {
            List<Module> modules = storeManifest.Where(x => (x.Value == null || x.Value.Contains(resource))
            && x.Key.isEnabled() && x.Key.mResourceStorageIndicator.mValue < 0.95f).Select(x => x.Key).ToList();

            // No valid stores found, send to any
            if (modules.Count == 0)
            {
                modules = storeManifest.Keys.ToList();
            }

            return modules;
        }

        public int GetAllModuleCount()
        {
            return storeManifest.Count;
        }

        public int GetValidModuleCount(Type resource)
        {
            return GetValidModules(resource).Count();
        }

        public bool IsStorageAvailable(Type resource)
        {
            var modules = storeManifest.Where(x => x.Value.Contains(resource)).ToList();
            return modules.Count > 0;
        }

        public Module FindNearestModule(Vector3 position, List<Module> validModules)
        {
            try
            {
                if (validModules != null && validModules.Count > 0 && position != null)
                {
                    var orderedModules = validModules.OrderBy(x => x.distanceTo(position)).ToList();

                    if (orderedModules != null && orderedModules.Count > 0)
                    {
                        return orderedModules[0];
                    }
                }
            }
            catch(NullReferenceException)
            {
            }

            return null;
        }

        public Module GetModuleById(int id)
        {
            var storage = GetAllStorage();

            return storage == null ? null : storage.FirstOrDefault(x => x.getId() == id);
        }

        public List<Module> GetAllStorage()
        {
            var modules = Module.getCategoryModules(Module.Category.Storage);

            if (modules != null)
            {
                for (int i = 0; i < modules.Count; i++)
                {
                    if (modules[i] == null)
                    {
                        modules.RemoveAt(i);
                    }
                }
            }
            return modules;
        }
    }    
}
