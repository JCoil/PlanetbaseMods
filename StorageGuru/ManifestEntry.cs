using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StorageGuru
{    
    public class ManifestEntry
    {
        public Module StorageModule;
        public List<ResourceType> AllowedResources;

        public int ModuleId => StorageModule.getId();

        public ManifestEntry(Module module, List<ResourceType> resourceTypes)
        {
            StorageModule = module;
            AllowedResources = resourceTypes;
        }

        #region Collection

        public void AddResourceDefinitions(params ResourceType[] resources)
        {
            foreach (var resource in resources)
            {
                if (resource != null && !ContainsResource(resource))
                {
                    AllowedResources.Add(resource);
                }
            }
        }

        public void RemoveResourceDefinitions(params ResourceType[] resources)
        {
            foreach (var resource in resources)
            {
                if (ContainsResource(resource))
                {
                    AllowedResources.RemoveAll(x => ResourceTypeComparer.Comparer.Equals(x, resource));
                }
            }
        }

        public void Clear()
        {
            AllowedResources = new List<ResourceType>();
        }

        public bool ContainsResource(ResourceType resource)
        {
            if (resource != null)
            {
                return AllowedResources.Contains(resource, ResourceTypeComparer.Comparer);
            }

            return false;
        }

        public int Count => AllowedResources.Count;

        #endregion

        #region Serilization

        public const char SerializeResourceSeparator = '|';

        [Obsolete("No longer required - saving handled by Game's xml serialization")]
        public string Serialize()
        {
            // Example 1234:[Metal|Food|Ore]
            return ModuleId + ":["
                + string.Join(SerializeResourceSeparator.ToString(), AllowedResources.Select(x => SerializeResource(x)).ToArray()) + "]";
        }

        [Obsolete("No longer required - saving handled by Game's xml serialization")]
        private string SerializeResource(ResourceType resourceType)
        {
            return resourceType.getName();
        }

        public class Blueprint
        {
            public int ModuleId;
            public List<string> Resources;

            public Blueprint(int id, List<string> resources)
            {
                ModuleId = id;
                Resources = resources;
            }

            [Obsolete("No longer required - saving handled by Game's xml serialization")]
            public static Blueprint Deserialize(string contents)
            {
                if (contents.Split(':') is string[] s && s.Length == 2 && int.TryParse(s[0], out int id))
                {
                    return new Blueprint(id, s[1].Replace("[", "").Replace("]", "").Split(SerializeResourceSeparator).ToList());
                }

                return null;
            }
        }

        #endregion
    }
}
