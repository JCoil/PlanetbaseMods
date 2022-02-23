using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace QoLTweaks
{
    public class QoLTweaks : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new QoLTweaks(), modEntry, "QoLTweaks");

        const int NewMedicalCabinetCapacity = 12;

        public override void OnInitialized()
        {
            SetMedicalCabinetCapacity();
            MedicalCabinetsInLabs();
        }

        private void SetMedicalCabinetCapacity()
        {
            // Update ResourceType definition
            TypeList<ComponentType, ComponentTypeList>.find<MedicalCabinet>().mEmbeddedResourceCount = NewMedicalCabinetCapacity;

            // Update all existing cabinets
            foreach (var component in ConstructionComponent.mComponents)
            {
                if (component.getComponentType() is MedicalCabinet cabinet)
                {
                    component.mResourceContainer.setCapacity(NewMedicalCabinetCapacity);
                }
            }
        }

        private void MedicalCabinetsInLabs()
        {
            // Add cabinet component to ModuleTypeLab
            var labComponents = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeLab>().mComponentTypes.ToList();
            labComponents.Add(TypeList<ComponentType, ComponentTypeList>.find<MedicalCabinet>());
            TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeLab>().mComponentTypes = labComponents.ToArray();

            // Update existing labs to be recognised as having storage components
            foreach (var module in Module.mModules)
            {
                if (module.getModuleType() is ModuleTypeLab)
                {
                    Module.mModuleCategories[(int)Module.Category.StorageComponentContaner].Add(module);
                }
            }
        }

        public override void Update(float timeStep)
        {
            // Nothing required here
        }
    }
}
