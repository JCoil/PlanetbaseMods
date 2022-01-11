using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImprovedComponents
{
	public class GmMedicinalPad : ComponentType
	{
		public const string Name = "GM Medicinal Pad";
		public const string Description = "A vegetable pad for fast-growing Medicinal Plants.";

		public GmMedicinalPad()
		{
			this.mConstructionCosts = new ResourceAmounts();
			this.mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 1);

			this.mIcon = loadIcon();

			base.addResourceProduction<MedicinalPlants>(ResourceSubtype.None);

			this.mResourceProductionPeriod = 405f;
			this.mConditionDecayTime = 720f;
			this.mPowerGeneration = -500;
			this.mWaterGeneration = -250;

			this.mRadius = 0.875f;
			this.mHeight = 1f;
			this.mRepairTime = 180f;

			this.mFlags |= 4194304;
			this.mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Biologist>();
			base.addUsageAnimation(CharacterAnimationType.WorkPadHigh, CharacterProp.Count, CharacterProp.Count);
			base.addUsageAnimation(CharacterAnimationType.TakeNotes, CharacterProp.Pad, CharacterProp.Count);
			this.initStrings();

			this.mRequiredTech = TypeList<Tech, TechList>.find<TechGmMedicinalPlants>();
			this.mPrefabName = "PrefabMedicinalPad";
		}

		public new Texture2D loadIcon()
		{
			return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.MedicinalPadTypeName, Color.white);
		}

		public static void RegisterStrings()
		{
			StringList.mStrings.Add("component_gm_medicinal_pad", Name);
			StringList.mStrings.Add("tooltip_gm_medicinal_pad", Description);
		}
	}
}
