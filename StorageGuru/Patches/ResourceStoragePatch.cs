using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace StorageGuru
{
    [HarmonyPatch(typeof(ResourceStorage))]
	public static class ResourceStoragePatch
	{
        public static bool UseImprovedSlotLayout { get; set; } = true;

        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(Vector3), typeof(float) })]
		public static void Postfix(Vector3 modulePosition, float radius, ref List<StorageSlot> ___mSlots)
        {
            if (UseImprovedSlotLayout)
            {
                ___mSlots.Clear();
                GenerateSlots_Spherical(___mSlots, modulePosition, radius);
            }
        }

        private static void GenerateSlots_Spherical(List<StorageSlot> slots, Vector3 modulePosition, float radius)
        {
            radius -= 2f; // Old useable radius was calculated as radius * 0.6

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
        }

        public static void ResourceStorage(Vector3 modulePosition, float radius, List<StorageSlot> ___mSlots)
		{
			// Creates list (2D grid) of square columns ("slots") that fit within a circle
			// Heights determining how many resources can be stacked, using linear interpolation, giving us a "cone" shaped storage space (with a 0.5 cyclinder at the base
			// We can do much better
			// 1) The 0.6*radius means that we miss a lot of space for larger domes - better to use a fixed margin for charcaters to traverse
			// 2) The cone wastes a lot of space, so we'll switch to a proper hemisphere
			// 3) need to update the storage full warnings too

			// Default sizes are: 5, 6.25, 7.5, 8.75
			// Old sizes are: 3, 3.75, 4.5, 5.25
			// New sizes are: 3, 4.25, 5.5, 6.75  
		}
	}
}
