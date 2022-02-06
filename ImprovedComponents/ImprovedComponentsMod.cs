using Planetbase;
using PlanetbaseModUtilities;
using System.Linq;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace ImprovedComponents
{
    public class ImprovedComponentsMod : ModBase
    {
        public static void Init(ModEntry modEntry) => InitializeMod(new ImprovedComponentsMod(), modEntry, "ImprovedComponents");

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
            var techList = TechList.get();

            techList.Add(new TechImprovedProcessing());
            techList.Add(new TechGmRice());
            techList.Add(new TechGmWheat());
            techList.Add(new TechGmMedicinalPlants());
        }

        private void RegisterNewComponents()
        {
            // Register new components to global lists
            var componentTypeList = TypeList<ComponentType, ComponentTypeList>.get();

            componentTypeList.Add(new EnhancedMetalProcessor());
            componentTypeList.Add(new EnhancedBioplasticProcessor());
            componentTypeList.Add(new GmRicePad());
            componentTypeList.Add(new GmWheatPad());
            componentTypeList.Add(new GmMedicinalPad());

            // Add new components to processing plants
            var processingPlantType = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeProcessingPlant>();
            var processingPlantComponents = BuildableUtils.GetComponentTypes(processingPlantType);

            processingPlantComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<EnhancedBioplasticProcessor>());
            processingPlantComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<EnhancedMetalProcessor>());

            BuildableUtils.SetComponentTypes(processingPlantType, processingPlantComponents);

            return;
            // Add new components to bio domes
            var bioDomType = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeBioDome>();
            var bioDomeComponents = bioDomType.getComponentTypes(bioDomType.getDefaultSize()).ToList();

            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmRicePad>());
            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmWheatPad>());
            bioDomeComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<GmMedicinalPad>());
        }

        public override void OnGameStart()
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {        
            // Nothing required here
        }
    }
}
