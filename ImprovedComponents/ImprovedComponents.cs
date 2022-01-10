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
    public class ImprovedComponents : IMod
    {
        public void Init()
        {
            TechImprovedProcessing.RegisterStrings();
            ImprovedMetalProcessor.RegisterStrings();
            ImprovedBioplasticProcessor.RegisterStrings();

            RegisterNewTechs();
            RegisterNewComponents();

            Debug.Log("[MOD] ImprovedComponents activated");
        }

        private static void RegisterNewTechs()
        {
            TypeList<Tech, TechList>.getInstance().add(new TechImprovedProcessing());
        }

        private static void RegisterNewComponents()
        {
            // Register new components to global lists
            TypeList<ComponentType, ComponentTypeList>.getInstance().add(new ImprovedMetalProcessor());
            TypeList<ComponentType, ComponentTypeList>.getInstance().add(new ImprovedBioplasticProcessor());

            // Add new components to processing plants
            var processingPlantComponents = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeProcessingPlant>().mComponentTypes.ToList();

            processingPlantComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<ImprovedBioplasticProcessor>());
            processingPlantComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<ImprovedMetalProcessor>());

            TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeProcessingPlant>().mComponentTypes = processingPlantComponents.ToArray();
        }

        public void Update()
        {
            // Nothing required here
        }
    }
}
