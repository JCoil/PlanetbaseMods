using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace ImprovedComponents
{
    public class GmRicePad : ComponentType
	{
		public const string Name = "GM Rice Pad";
		public const string Description = "A hydroponic vegetable pad";
 
		public GmRicePad()
		{		
			this.mConstructionCosts = new ResourceAmounts();
			this.mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 1);

			this.mIcon = loadIcon();

			base.addResourceProduction<Vegetables>(ResourceSubtype.Rice);
			base.addResourceProduction<Vegetables>(ResourceSubtype.Rice);
			base.addResourceProduction<Starch>(ResourceSubtype.None);
			base.addResourceProduction<Starch>(ResourceSubtype.None);

			this.mResourceProductionPeriod = 810f;
			this.mConditionDecayTime = 1080f;
			this.mPowerGeneration = -500;
			this.mWaterGeneration = -250;

			this.mRadius = 0.875f;
			this.mHeight = 1f;
			this.mRepairTime = 180f;

			this.mFlags |= 4194304;
			this.mFlags |= 2048;
			this.mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Biologist>();
			base.addUsageAnimation(CharacterAnimationType.WorkPadLow, CharacterProp.Count, CharacterProp.Count);
			base.addUsageAnimation(CharacterAnimationType.TakeNotes, CharacterProp.Pad, CharacterProp.Count);

			this.initStrings();

			// Need to borrow tooltip and prefab from RicePad because they are not generated correctly by initStrings
			this.mTooltip = TypeList<ComponentType, ComponentTypeList>.find<RicePad>().getTooltip();
			this.mPrefabName = "PrefabPadStarchyRice";

			this.mRequiredTech = TypeList<Tech, TechList>.find<TechGmRice>();
		}

		public new Texture2D loadIcon()
		{
			return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.RicePadTypeName,
				TypeList<ResourceType, ResourceTypeList>.find<Starch>().getStatsColor());
		}

		public static void RegisterStrings()
		{
			StringUtils.GlobalStrings.Add("component_gm_rice_pad", Name);
			StringUtils.GlobalStrings.Add("tooltip_gm_rice_pad", Description);
		}
	}
}
