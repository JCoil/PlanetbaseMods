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
	[HarmonyPatch(typeof(Module))]
	public class StorageModulePatches
	{
		/// <summary>
		/// Because of the way this is done, by picking the first module that has free slots and not passing the resource type to 
		/// getEmptyStorageSlotCount, the easiest way I can see to inject our code here is essentially replacing the method
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(nameof(Module.findStorage))]
		public static bool FindStoragePrefix(ref Module __result, Character character)
		{
			float smallestDistance = float.MaxValue;
			__result = null;
			Vector3 position = character.getPosition();

			List<Module> storageModules = Module.getCategoryModules(Module.Category.Storage);

			if (character.getLoadedResource() is Resource resource)
			{
				// Shortlist only modules that allow this resource
				storageModules = ImprovedResourceStorage.GetValidModules(storageModules, resource.getResourceType());
			}

			if (storageModules != null)
			{
				int count = storageModules.Count;
				for (int i = 0; i < count; i++)
				{
					Module module = storageModules[i];
					if (module.getGameObject() != null && module.isOperational() && module.isSurvivable(character) && module.getEmptyStorageSlotCount() > module.getPotentialUserCount(character))
					{
						float sqrMagnitude = (module.getPosition() - position).sqrMagnitude;
						if (sqrMagnitude < smallestDistance)
						{
							__result = module;
							smallestDistance = sqrMagnitude;
						}
					}
				}
			}

			return false; // We don't want to run the original code
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Module), nameof(Module.onBuilt))]
		public static void OnBuiltPostfix(ref Module __instance, ref ResourceStorage ___mResourceStorage)
		{
			if (__instance.hasFlag(8) && ___mResourceStorage is ResourceStorage)
			{
				if (!(___mResourceStorage is ImprovedResourceStorage)) // This means it must be newly built
				{
					___mResourceStorage = new ImprovedResourceStorage(__instance.getFloorPosition(), __instance.getRadius());
				}
			}
		}

		/// <summary>
		/// Add check for storage manifest on serialize if exists
		/// </summary>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Module), nameof(Module.serialize))]
		public static void SerializePostfix(ref Module __instance, XmlNode parent, ref ResourceStorage ___mResourceStorage)
		{
			if (___mResourceStorage is ImprovedResourceStorage improvedResourceStorage)
			{
				XmlNode storageGuruNode = Serialization.createNode(parent.LastChild, "storageguru-manifest");

				Serialization.serializeBool(storageGuruNode, "has-improved-slot-layout", improvedResourceStorage.HasImprovedSlotLayout);
				Serialization.serializeList(storageGuruNode, "resource-types", improvedResourceStorage.GetAllowedResourceStringList());
			}
		}

		/// <summary>
		/// Deserialize manifest xml node if exists
		/// </summary>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Module), nameof(Module.deserialize))]
		public static void DeserializePostfix(ref Module __instance, XmlNode node, ref ResourceStorage ___mResourceStorage)
		{
			if (___mResourceStorage != null)
			{
				var resourceStorage = new ImprovedResourceStorage();
				resourceStorage.CopyFrom(___mResourceStorage);

				if (node["storageguru-manifest"] is XmlNode manifestNode)
				{
					var resourceTypes = new List<ResourceType>();
					var hasImprovedSlotLayout = Serialization.deserializeBool(manifestNode["has-improved-slot-layout"]);

					if (Serialization.deserializeList<string>(manifestNode["resource-types"]) is List<string> resources)
					{
						foreach (var resourceName in resources)
						{
							if (TypeList<ResourceType, ResourceTypeList>.find(resourceName) is ResourceType resourceType)
							{
								resourceTypes.Add(resourceType);
							}
						}
					}

					resourceStorage.SetManifest(resourceTypes);
					resourceStorage.HasImprovedSlotLayout = hasImprovedSlotLayout;
				}

				___mResourceStorage = resourceStorage;
			}
		}

		/// <summary>
		/// If a storage is empty, we can update to the improved layout
		/// </summary>
		/// <param name="__instance"></param>
		/// <param name="___mResourceStorage"></param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Module), nameof(Module.update))]
		public static void UpdatePostfix(Module __instance, ref ResourceStorage ___mResourceStorage)
		{
			if (___mResourceStorage is ImprovedResourceStorage improvedResourceStorage 
				&& improvedResourceStorage.GetTotalResourceCount() == 0 
				&& !improvedResourceStorage.HasImprovedSlotLayout)
			{
				improvedResourceStorage.UpdateSlotLayout(__instance.getFloorPosition(), __instance.getRadius());
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Module), nameof(Module.getName))]
		public static void GetNamePostfix(ref ResourceStorage ___mResourceStorage, ref string __result)
		{
			if (___mResourceStorage is ImprovedResourceStorage improvedResourceStorage && improvedResourceStorage.HasImprovedSlotLayout)
			{
				__result = "Improved Storage";
			}
		}
	}
}
