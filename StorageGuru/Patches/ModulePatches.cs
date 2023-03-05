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
{/// <summary>
 /// Because of the way this is done, by picking the first module that has free slots and not passing the resource type to 
 /// getEmptyStorageSlotCount, the easiest way I can see to inject our code here is essentially replacing the method
 /// </summary>
    [HarmonyPatch(typeof(Module), nameof(Module.findStorage))]
    public class ModuleFindStoragePatch
    {
        public static bool Prefix(ref Module __result, Character character)
		{
			float smallestDistance = float.MaxValue;
			__result = null;
			Vector3 position = character.getPosition();

			List<Module> storageModules = Module.getCategoryModules(Module.Category.Storage);

			if (character.getLoadedResource() is Resource resource)
			{
				// Shortlist only modules that allow this resource
				storageModules = StorageGuruMod.StorageController.ListValidModules(resource.getResourceType());
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
    }

    [HarmonyPatch(typeof(Module), nameof(Module.serialize))]
	public class ModuleSerializePatch
    {
		/// <summary>
		/// Add check for storage manifest an serialize if exists
		/// </summary>
		public static void Postfix(ref Module __instance, XmlNode parent)
		{
			if(StorageGuruMod.StorageController.GetManifestEntry(__instance) is ManifestEntry entry)
            {
                XmlNode storageGuruNode = Serialization.createNode(parent.LastChild, "storageguru-manifest");

				Serialization.serializeList(storageGuruNode, "resource-types", entry.AllowedResources.Select(x => x.GetType().Name).ToList());
			}
		}
	}

	[HarmonyPatch(typeof(Module), nameof(Module.deserialize))]
	public class ModuleDeserializePatch
	{
		/// <summary>
		/// Deserialize manifest xml node if exists
		/// </summary>
		public static void Postfix(ref Module __instance, XmlNode node)
		{
            if (node["storageguru-manifest"] is XmlNode manifestNode)
            {
                var resourceTypes = new List<ResourceType>();

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

                StorageGuruMod.StorageController.AddManifestEntry(__instance, resourceTypes);
            }
		}
	}

	[HarmonyPatch(typeof(Module), "createIndicators")]
	public class ModuleCreateIndicatorsPatch
	{
		/// <summary>
		/// Add check for storage manifest an serialize if exists
		/// </summary>
		public static void Postfix(ref Indicator ___mResourceStorageIndicator)
		{
			if (___mResourceStorageIndicator != null)
			{
				CoreUtils.SetMember("mType", ___mResourceStorageIndicator, IndicatorType.Normal);
			}
		}
	}

	[HarmonyPatch(typeof(Module), "tick")]
	public class ModuleTickPatch
	{
		/// <summary>
		/// Add check for storage manifest an serialize if exists
		/// </summary>
		public static void Postfix(ref ResourceStorage ___mResourceStorage, ref Indicator ___mResourceStorageIndicator)
		{
			// Original code at end of tick
			//if (this.mResourceStorage! = null)
			//{
			//	this.mResourceStorageIndicator.setValue(this.mResourceStorage.getSpaceRatio());
			//}

			if(___mResourceStorage != null)
            {
				float resourceVolume = 0f;
				float totalVolume = 0f;

				foreach (StorageSlot storageSlot in ___mResourceStorage.GetSlots())
				{
					float maxHeight = storageSlot.getMaxHeight();
					resourceVolume += Mathf.Min(storageSlot.getHeight() - 0.2f, maxHeight);
					totalVolume += maxHeight;
				}

				___mResourceStorageIndicator.setMax(totalVolume);
				___mResourceStorageIndicator.setValue(resourceVolume);
            }
		}
	}
}
