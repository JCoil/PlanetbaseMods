using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;

namespace QoLTweaks
{
    public class QoLTweaks : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new QoLTweaks(), modEntry, "QoLTweaks");

        const int NewMedicalCabinetCapacity = 12;

        public override void OnInitialized(ModEntry modEntry)
        {
            // Change base behaviour of game elements
            SetDefaultMedicalCabinetCapacity();
            AllowMedicalCabinetsInLabs();
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            // Update existing elements to match new base behavious
            UpdateExistingMedicalCabinets();
            UpdateExistingLabsToAllowMedicalCabinets();
        }

        #region OnInitialized

        private void SetDefaultMedicalCabinetCapacity()
        {
            CoreUtils.SetMember("mEmbeddedResourceCount", ComponentTypeList.find<MedicalCabinet>(), NewMedicalCabinetCapacity);
        }

        private void AllowMedicalCabinetsInLabs()
        {
            // Add cabinet component to ModuleTypeLab 
            // getCategory() will now return Module.Category.StorageComponentContaner for labs so they will be added to the master list on init
            var labComponents = ModuleTypeList.find<ModuleTypeLab>().GetComponentTypes();
            labComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<MedicalCabinet>());
            ModuleTypeList.find<ModuleTypeLab>().SetComponentTypes(labComponents);
        }

        #endregion

        #region OnGameStart

        private static void UpdateExistingMedicalCabinets()
        {
            foreach (var component in BuildableUtils.GetAllComponents())
            {
                if (component.getComponentType() is MedicalCabinet && component.getResourceContainer() is ResourceContainer container)
                {
                    container.setCapacity(NewMedicalCabinetCapacity);
                }
            }
        }

        private void UpdateExistingLabsToAllowMedicalCabinets()
        {
            // Update existing labs to be recognised as having storage components
            foreach (var module in BuildableUtils.GetAllModules())
            {
                if (module.getModuleType() is ModuleTypeLab)
                {
                    module.SetCategory(Module.Category.StorageComponentContaner); // Typo in Planetbase assembly
                }
            }
        }

        #endregion

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing required here for now
        }
    }
}
