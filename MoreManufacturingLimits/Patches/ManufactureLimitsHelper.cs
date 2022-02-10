using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImprovedManufacturingLimits
{
	public static class ManufactureLimitsHelper
	{
		public static Dictionary<ResourceType, RefInt> ResourceLimits =>
			CoreUtils.GetMember<ManufactureLimits, Dictionary<ResourceType, RefInt>>("mResourceLimits", Singleton<ManufactureLimits>.getInstance());

		public static Dictionary<Specialization, RefInt> BotLimits =>
			CoreUtils.GetMember<ManufactureLimits, Dictionary<Specialization, RefInt>>("mBotLimits", Singleton<ManufactureLimits>.getInstance());

		public static bool IsUnderLimit(ResourceType resourceType)
		{
			if (resourceType != null && ResourceLimits.TryGetValue(resourceType, out RefInt refInt))
			{
				return Resource.getTotalAmounts().getAmount(resourceType) < refInt.get();
			}

			return true;
		}

		public static bool IsUnderLimit(ComponentType componentType)
		{
			if (componentType.getProduction() != null && componentType.getProduction().Count > 0)
			{
				bool overLimit = true;

				foreach (var productionItem in componentType.getProduction())
				{
					if (productionItem.getResourceType() is ResourceType resourceType)
					{
						overLimit &= !IsUnderLimit(resourceType);
					}
					else if (productionItem.getBotType() is Specialization botType)
					{
						overLimit &= !IsUnderLimit(botType);
					}
				}

				return !overLimit;
			}
			return true;
		}

		public static bool IsUnderLimit(ConstructionComponent component) => IsUnderLimit(component.getComponentType());		

		public static bool IsUnderLimit(Specialization botType) => Character.getCountOfSpecialization(botType) < BotLimits[botType].get();

		public static bool IsUnderLimit<T>() where T : ResourceType => IsUnderLimit(TypeList<ResourceType, ResourceTypeList>.find<T>());		
	}
}
