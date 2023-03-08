using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using PlanetbaseModUtilities;

namespace StorageGuru
{
    public class ImprovedResourceStorage : ResourceStorage
    {
        public bool HasImprovedSlotLayout { get; set; } = false;
        public ManifestEntry StorageManifest { get; private set; }

        // Annoyingly mSlots is private so we'll expose it here
        public List<StorageSlot> Slots
        {
            get => BuildableUtils.GetSlots(this);
            set => BuildableUtils.SetSlots(this, value);
        }

        public ImprovedResourceStorage(Vector3 modulePosition, float radius)
        {
            UpdateSlotLayout(modulePosition, radius);

            StorageManifest = new ManifestEntry(new List<ResourceType>());
            AddAllDefinitions(); // If it's a new build, we'll allow all resource types
        }

        public ImprovedResourceStorage(ResourceStorage resourceStorage, List<ResourceType> resourceTypes, bool hasImprovedSlotLayout) 
        {
            Slots.Clear();
            Slots.AddRange(resourceStorage.GetSlots());

            HasImprovedSlotLayout = hasImprovedSlotLayout;

            StorageManifest = new ManifestEntry(resourceTypes);
        }

        public void UpdateSlotLayout(Vector3 modulePosition, float radius)
        {
            Slots = GenerateSlots_Spherical(modulePosition, radius);
            HasImprovedSlotLayout = true;
        }

        public int GetTotalResourceCount() => (int)Slots.Sum(x => x.GetResources() == null? 0 : x.GetResources().Count); 
        public List<Resource> GetAllResources() => Slots.SelectMany(x => x.GetResources()).ToList();
        public int GetCountOfResource(ResourceType resourceType) => GetAllResources().Count(x => Equals(x, resourceType));

        public static List<Module> GetValidModules(List<Module> modules, ResourceType resourceType)
        {
            return modules.Where(
                x => x.GetResourceStorageObject() is ImprovedResourceStorage improvedResourceStorage
                && improvedResourceStorage.StorageManifest.ContainsResource(resourceType)).ToList();
        }

        #region Manifest Management

        public void ToggleDefinition(ResourceType resource)
        {
            if (StorageManifest.ContainsResource(resource))
            {
                StorageManifest.RemoveResourceDefinitions(resource);
            }
            else
            {
                StorageManifest.AddResourceDefinitions(resource);
            }

            RefreshStorage();
        }

        public void AddAllDefinitions()
        {
            StorageManifest.AddResourceDefinitions(StorageGuruMod.MasterResourceDefinitions.ToArray());
            RefreshStorage();
        }

        public void RemoveAllDefinitions()
        {
            StorageManifest = new ManifestEntry(new List<ResourceType>());
            RefreshStorage();
        }

        private void RefreshStorage()
        {
            foreach (var slot in Slots)
            {
                if (slot != null && slot.GetResources() != null)
                {
                    foreach (var resource in slot.GetResources())
                    {
                        if (StorageManifest.ContainsResource(resource.getResourceType()))
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

        public List<string> GetAllowedResourceStringList()
        {
            return StorageManifest.AllowedResources.Select(x => x.GetType().Name).ToList();
        }

        #endregion

        #region Slots factories

        private static List<StorageSlot> GenerateSlots_Spherical(Vector3 modulePosition, float radius)
        {
            var slots = new List<StorageSlot>();
            radius *= 0.9f;
            radius -= 1.5f; // Old useable radius was calculated as simply radius * 0.6

            var increment = 1.15f; // Size of resources (1) + gap (0.15)

            for (float z = increment / 2f; z <= radius; z += increment) // z varies from 0 to R
            {
                var width_squared = radius * radius - z * z;
                var width = Math.Sqrt(width_squared); // Width of circle in x-direction at z

                for (float x = increment / 2f; x <= width; x += increment) // Only need to check for x between 0 and width
                {
                    var height_squared = width_squared - x * x;
                    var height = (int)(Math.Round(Math.Sqrt(height_squared) * 2) / 2); // Round to the nearest 0.5 for half-sized resources

                    // Add slot for each quadrant
                    slots.Add(new StorageSlot(modulePosition + new Vector3(x, 0f, z), height));
                    slots.Add(new StorageSlot(modulePosition + new Vector3(-x, 0f, z), height));
                    slots.Add(new StorageSlot(modulePosition + new Vector3(x, 0f, -z), height));
                    slots.Add(new StorageSlot(modulePosition + new Vector3(-x, 0f, -z), height));
                }
            }
            return slots;
        }

        [Obsolete("Original slot generation code. New version is GenerateSlots_Spherical")]
        private static List<StorageSlot> GenerateSlots_Conical(Vector3 modulePosition, float radius)
        {
            var slots = new List<StorageSlot>();

            float num = radius * 0.6f;
            float num2 = radius * 1.5f;
            for (float num3 = -num2; num3 <= num2; num3 += 1.15f)
            {
                for (float num4 = -num2; num4 <= num2; num4 += 1.15f)
                {
                    Vector3 b = new Vector3(num3, 0f, num4);
                    float magnitude = b.magnitude;
                    if (magnitude < num)
                    {
                        slots.Add(new StorageSlot(modulePosition + b, 0.5f + (num - magnitude)));
                    }
                }
            }

            return slots;
        }

        #endregion
    }
}
