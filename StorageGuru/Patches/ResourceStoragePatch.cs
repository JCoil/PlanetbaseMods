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
	[HarmonyPatch(typeof(ResourceStorage), MethodType.Constructor, new Type[] {typeof(Vector3), typeof(float)})]
	public static class ResourceStoragePatch
	{
		public static void Postfix(Vector3 modulePosition, float radius, ref List<StorageSlot> ___mSlots)
		{
			if(___mSlots == null)
            {
				___mSlots = new List<StorageSlot>();
            }

			___mSlots.Clear();

			radius -= 2f;

			for (float z = -radius; z <= radius; z += 1.15f)
			{
				var currentRSquared = radius * radius - z * z;
				var currentR = Math.Sqrt(currentRSquared);

				for (float x = -radius; x <= radius; x += 1.15f)
				{
					Vector3 slotPosition = new Vector3(x, 0f, z);

					float distanceFromCenter = slotPosition.magnitude;
					var height = currentR * Math.Sqrt(1 - (x * x / currentRSquared));

					if (distanceFromCenter < radius)
					{
						___mSlots.Add(new StorageSlot(modulePosition + slotPosition, (float)height));
					}
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
