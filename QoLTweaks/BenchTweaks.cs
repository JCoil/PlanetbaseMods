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
        public static void AllowBenchesInBiodomes()
        {
            // Add cabinet component to ModuleTypeLab 
            // getCategory() will now return Module.Category.StorageComponentContaner for labs so they will be added to the master list on init
            var bioDomeComponents = ModuleTypeList.find<ModuleTypeBioDome>().GetComponentTypes();
            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<Bench>());
            ModuleTypeList.find<ModuleTypeBioDome>().SetComponentTypes(bioDomeComponents);
        }
    }
}
