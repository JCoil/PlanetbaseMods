using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImprovedComponents
{
    public class ImprovedComponentsMod : IMod
    {
        public void Init()
        {
            RegisterStrings();
            RegisterNewTechs();
            RegisterNewComponents();

            Debug.Log("[MOD] ImprovedComponents activated");
        }

        private static void RegisterStrings()
        {
            TechImprovedProcessing.RegisterStrings();
            TechGmRice.RegisterStrings();
            TechGmWheat.RegisterStrings();
            TechGmMedicinalPlants.RegisterStrings();

            EnhancedMetalProcessor.RegisterStrings();
            EnhancedBioplasticProcessor.RegisterStrings();
            GmRicePad.RegisterStrings();
            GmWheatPad.RegisterStrings();
            GmMedicinalPad.RegisterStrings();
        }

        private static void RegisterNewTechs()
        {
            var techList = TypeList<Tech, TechList>.getInstance();

            techList.add(new TechImprovedProcessing());
            techList.add(new TechGmRice());
            techList.add(new TechGmWheat());
            techList.add(new TechGmMedicinalPlants());
        }

        private static void RegisterNewComponents()
        {
            // Register new components to global lists
            var componentTypeList = TypeList<ComponentType, ComponentTypeList>.getInstance();

            componentTypeList.add(new EnhancedMetalProcessor());
            componentTypeList.add(new EnhancedBioplasticProcessor());
            componentTypeList.add(new GmRicePad());
            componentTypeList.add(new GmWheatPad());
            componentTypeList.add(new GmMedicinalPad());

            // Add new components to processing plants
            var processingPlantComponents = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeProcessingPlant>().mComponentTypes.ToList();

            processingPlantComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<EnhancedBioplasticProcessor>());
            processingPlantComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<EnhancedMetalProcessor>());

            TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeProcessingPlant>().mComponentTypes = processingPlantComponents.ToArray();

            // Add new components to bio domes
            var bioDomeComponents = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeBioDome>().mComponentTypes.ToList();

            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmRicePad>());
            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmWheatPad>());
            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmMedicinalPad>());

            TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeBioDome>().mComponentTypes = bioDomeComponents.ToArray();
        }

        public void Update()
        {
            // Nothing required here
        }
    }
}
