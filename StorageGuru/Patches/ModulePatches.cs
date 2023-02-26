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
}
