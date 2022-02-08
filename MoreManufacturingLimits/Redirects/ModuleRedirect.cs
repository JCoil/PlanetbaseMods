using Planetbase;
using Redirection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ImprovedManufacturingLimits
{
	public class ModuleRedirect : Module
	{
		/// <summary>
		/// Need to handle mines directly because they use their own Ai
		/// </summary>
		[RedirectFrom(typeof(Module))]
		public static new Module findMine(Character character, bool highPriority, int maxTargeters)
		{
			float num = float.MaxValue;
			Module result = null;
			Vector3 position = character.getPosition();
			List<Module> list = Module.getCategoryModules(Category.Mine);
			if (list != null && ManufactureLimitsHelper.isUnderLimit(TypeList<ResourceType, ResourceTypeList>.find<Ore>()))
			{
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					Module module = list[i];
					if ((!highPriority || module.isHighPriority()) && module.isOperational() && module.getPotentialUserCount(character) <= maxTargeters)
					{
						float sqrMagnitude = (module.getPosition() - position).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							result = module;
							num = sqrMagnitude;
						}
					}
				}
			}
			return result;
		}

		private static bool IsUnderLimit(ResourceType resourceType)
		{
			RefInt refInt;
			if (resourceType != null &&  ManufactureLimitsHelper.ResourceLimits.TryGetValue(resourceType, out refInt))
			{
				return Resource.getTotalAmounts().getAmount(resourceType) < refInt.get();
			}

			return true;
		}
	}
}
