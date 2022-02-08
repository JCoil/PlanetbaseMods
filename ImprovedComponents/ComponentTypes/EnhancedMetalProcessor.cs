using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using UnityEngine;

namespace ImprovedComponents
{
    public class EnhancedMetalProcessor : ComponentType
	{
        public const string Name = "Electrolytic Reduction Processor";
        public const string Description = "Efficiently creates Metal from Ore.";

        public EnhancedMetalProcessor()
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
            this.mResourceProductionPeriod = 180f;
            this.mPowerGeneration = -2000;
            this.mWaterGeneration = -500;

            this.mFlags = 1572904;
            this.mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            base.addUsageAnimation(CharacterAnimationType.WorkStanding, CharacterProp.Count, CharacterProp.Count);
            this.initStrings();

            this.mRequiredTech = TypeList<Tech, TechList>.find<TechImprovedProcessing>();
            this.mPrefabName = "PrefabMetalProcessor";
        }

        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.MetalProcessorTypeName);
        }

        public static void RegisterStrings()
        {
            StringUtils.GlobalStrings.Add("component_enhanced_metal_processor", Name);
            StringUtils.GlobalStrings.Add("tooltip_enhanced_metal_processor", Description);
        }
    }
}
