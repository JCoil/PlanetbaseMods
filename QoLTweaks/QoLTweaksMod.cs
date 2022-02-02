using ModWrapper;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QoLTweaks
{
    public class QoLTweaksMod : ModBase, IMod
    {
        const int NewMedicalCabinetCapacity = 12;

        public override void Init()
        {
            Debug.Log("[MOD] QoL Tweaks activated");
        }

        public override void OnGameStart()
        {
            SetMedicalCabinetCapacity();
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

        public override void Update(float timeStep)
        {
            // Nothing required here
        }
    }
}
