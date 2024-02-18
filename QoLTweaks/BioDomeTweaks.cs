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

            ModuleTypeList.find<ModuleTypeBioDome>().SetComponentTypes(bioDomeComponents);
        }
    }
}
