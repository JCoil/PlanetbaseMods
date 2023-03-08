using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StorageGuru
{    
    public struct ManifestEntry
    {
        public List<ResourceType> AllowedResources { get; private set; }

        public ManifestEntry(List<ResourceType> resourceTypes)
        {
            AllowedResources = resourceTypes;
        }

        public int Count => AllowedResources.Count;

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

        public bool ContainsResource(ResourceType resource)
        {
            if (resource != null)
            {
                return AllowedResources.Contains(resource, ResourceTypeComparer.Comparer);
            }

            return false;
        }

        #endregion
    }
}
