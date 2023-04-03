using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;
using Planetbase;

namespace QoLTweaks
{
    public static class MedicalCabinetTweaks
    {
        const int NewMedicalCabinetCapacity = 12;

        public static void UpdateMedicalCabinetCapacity()
        {
            CoreUtils.SetMember("mEmbeddedResourceCount", ComponentTypeList.find<MedicalCabinet>(), NewMedicalCabinetCapacity);
        }

        /// <summary>
        /// We also need to update the capacity of any cabinets that were placed before the mod was installed
        /// </summary>
        public static void UpdateExistingMedicalCabinetCapacity()
        {
            foreach (var component in BuildableUtils.GetAllComponents())
            {
                if (component.getComponentType() is MedicalCabinet && component.getResourceContainer() is ResourceContainer container)
                {
                    container.setCapacity(NewMedicalCabinetCapacity);
                }
            }
        }

        public static void AllowMedicalCabinetsInLabs()
        {
            // Add cabinet component to ModuleTypeLab 
            // getCategory() will now return Module.Category.StorageComponentContaner for labs so they will be added to the master list on init
            var labComponents = ModuleTypeList.find<ModuleTypeLab>().GetComponentTypes();
            labComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<MedicalCabinet>());
            ModuleTypeList.find<ModuleTypeLab>().SetComponentTypes(labComponents);
        }

        /// <summary>
        /// We also need to update category of any labs that were placed before the mod was installed
        /// </summary>
        public static void UpdateExistingAllowMedicalCabinetsInLabs()
        {
            // Update existing labs to be recognised as having storage components
            foreach (var module in BuildableUtils.GetAllModules())
            {
                if (module.getModuleType() is ModuleTypeLab || module.getModuleType() is ModuleTypeStorage)
                {
                    module.SetCategory(Module.Category.StorageComponentContaner); // Typo in Planetbase assembly
                }
            }
        }
    }
}
