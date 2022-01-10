using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImprovedComponents
{
    public class ImprovedMetalProcessor : ComponentType
	{
        public const string Name = "Enhanced Metal Processor";
        public const string Description = "Efficiently creates Metal from Ore.";

        public ImprovedMetalProcessor()
        {
            this.mConstructionCosts = new ResourceAmounts();
            this.mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
            this.mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>(), 1);
            this.mIcon = loadIcon();
            this.mResourceConsumption = new List<ResourceType>();
            this.mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<Ore>());
            base.addResourceProduction<Metal>(ResourceSubtype.None);
            base.addResourceProduction<Metal>(ResourceSubtype.None);
            this.mEmbeddedResourceCount = 3;
            this.mResourceProductionPeriod = 240f;
            this.mPowerGeneration = -3000;
            this.mFlags = 1572904;
            this.mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            base.addUsageAnimation(CharacterAnimationType.WorkStanding, CharacterProp.Count, CharacterProp.Count);
            this.initStrings();

            this.mRequiredTech = TypeList<Tech, TechList>.find<TechImprovedProcessing>();
            this.mPrefabName = "PrefabMetalProcessor";
        }

        public new Texture2D loadIcon(Color color)
        {
            return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.MetalProcessorTypeName, color);
        }

        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.MetalProcessorTypeName);
        }

        public static void RegisterStrings()
        {
            StringList.mStrings.Add("component_improved_metal_processor", Name);
            StringList.mStrings.Add("tooltip_improved_metal_processor", Description);
        }
    }
}
