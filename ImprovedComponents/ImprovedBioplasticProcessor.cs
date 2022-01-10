using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImprovedComponents
{
    public class ImprovedBioplasticProcessor : ComponentType
	{
		public const string Name = "Enhanced Bioplastic Processor";
		public const string Description = "Efficiently creates Bioplastic from Starch.";

		public ImprovedBioplasticProcessor()
		{
			this.mConstructionCosts = new ResourceAmounts();
			this.mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
			this.mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>(), 1);
			this.mIcon = loadIcon();
			this.mResourceConsumption = new List<ResourceType>();
			this.mResourceConsumption.Add(TypeList<ResourceType, ResourceTypeList>.find<Starch>());
			base.addResourceProduction<Bioplastic>(ResourceSubtype.None);
			base.addResourceProduction<Bioplastic>(ResourceSubtype.None);
			this.mEmbeddedResourceCount = 4;
			this.mResourceProductionPeriod = 200f;
			this.mPowerGeneration = -3000;
			this.mFlags = 1572904;
			this.mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
			base.addUsageAnimation(CharacterAnimationType.WorkStanding, CharacterProp.Count, CharacterProp.Count);
			this.initStrings();

			this.mRequiredTech = TypeList<Tech, TechList>.find<TechImprovedProcessing>();
			this.mPrefabName = "PrefabBioplasticProcessor";
		}

		public new Texture2D loadIcon(Color color)
		{
			return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.BioplasticProcessorTypeName, color);
		}

		public new Texture2D loadIcon()
		{
			return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.BioplasticProcessorTypeName);
		}

		public static void RegisterStrings()
		{
			StringList.mStrings.Add("component_improved_bioplastic_processor", Name);
			StringList.mStrings.Add("tooltip_improved_bioplastic_processor", Description);
		}
	}
}
