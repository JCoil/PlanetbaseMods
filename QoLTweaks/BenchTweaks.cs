using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;
using Planetbase;

namespace QoLTweaks
{
    public static class BenchTweaks
    {
        public static void AllowBenchesInBioDomes()
        {
            // Add bench component to ModuleTypeBioDome 
            var bioDomeComponents = ModuleTypeList.find<ModuleTypeBioDome>().GetComponentTypes();
            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<Bench>());
            ModuleTypeList.find<ModuleTypeBioDome>().SetComponentTypes(bioDomeComponents);
        }
    }
}
