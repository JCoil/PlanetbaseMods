using Planetbase;
using PlanetbaseModUtilities;
using System.Linq;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace ImprovedComponents
{
    public class ImprovedComponentsMod : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new ImprovedComponentsMod(), modEntry, "ImprovedComponents");

        public override void OnInitialized()
        {
            RegisterStrings();
            RegisterNewTechs();
            RegisterNewComponents();

            Debug.Log("[MOD] ImprovedComponents activated");
        }

        private void RegisterStrings()
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

        private void RegisterNewTechs()
        {
            var techList = TechList.getInstance();

            techList.AddType(new TechGmRice());
            techList.AddType(new TechGmWheat());
            techList.AddType(new TechGmMedicinalPlants());
            techList.AddType(new TechImprovedProcessing());
        }

        private void RegisterNewComponents()
        {
            // Register new components to global lists
            var componentTypeList = TypeList<ComponentType, ComponentTypeList>.getInstance();

            componentTypeList.AddType(new EnhancedMetalProcessor());
            componentTypeList.AddType(new EnhancedBioplasticProcessor());
            componentTypeList.AddType(new GmRicePad());
            componentTypeList.AddType(new GmWheatPad());
            componentTypeList.AddType(new GmMedicinalPad());

            // Add new components to processing plants
            var processingPlantType = BuildableUtils.FindModuleType<ModuleTypeProcessingPlant>();
            var processingPlantComponents = processingPlantType.GetComponentTypes();

            processingPlantComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<EnhancedBioplasticProcessor>());
            processingPlantComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<EnhancedMetalProcessor>());

            processingPlantType.SetComponentTypes(processingPlantComponents);

            // Add new components to bio domes
            var bioDomeType = BuildableUtils.FindModuleType<ModuleTypeBioDome>();
            var bioDomeComponents = bioDomeType.GetComponentTypes();

            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmRicePad>());
            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmWheatPad>());
            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmMedicinalPad>());

            bioDomeType.SetComponentTypes(bioDomeComponents);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {        
            // Nothing required here
        }
    }
}
