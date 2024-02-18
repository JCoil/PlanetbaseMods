using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;
using Planetbase;

namespace QoLTweaks
{
    public static class BioDomeTweaks
    {
        public static void AllowComponentsInBioDomes()
        {
            var bioDomeComponents = ModuleTypeList.find<ModuleTypeBioDome>().GetComponentTypes();

            if (QoLTweaks.Settings.BenchesInBioDomes)
            {
                bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<Bench>());
            }
        
            if (QoLTweaks.Settings.DrinkingFountainsInBioDomes)
            {
                bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<DrinkingFountain>());
            }

            if (QoLTweaks.Settings.DecorativePlantsInBioDomes)
            {
                bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<DecorativePlant>());
            }

            ModuleTypeList.find<ModuleTypeBioDome>().SetComponentTypes(bioDomeComponents);
        }
    }
}
