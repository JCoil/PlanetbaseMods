using System;
using UnityEngine;
using Planetbase;

namespace MoreManufacturingLimits
{
	public class GuiManufactureLimitsWindowImproved : GuiWindow
	{
		public GuiManufactureLimitsWindowImproved() : base(new GuiLabelItem(StringList.get("manufacture_limits"), TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>().getIcon(), null, 0, FontSize.Normal), null, null)
		{
			this.mHelpId = "manufacture_limits";

			// Manufactured Items
			var sectionManufacturedResources = new GuiSectionItem(StringList.get("manufacture_limits_manufactured"));
			var sectionRawResources = new GuiSectionItem(StringList.get("manufacture_limits_raw"));

			foreach (var element in Singleton<ManufactureLimits>.getInstance().mResourceLimits)
			{
				var resourceType = element.Key;
				var limit = element.Value;

				var tooltip = StringList.get("tooltip_manufacture_limit", element.Key.getNamePlural());
				var guiLabelItem = new GuiLabelItem(resourceType.getNamePlural(), resourceType.getIcon(), tooltip, 0, FontSize.Normal);

				var guiAmountSelector = new GuiAmountSelectorImproved(0, 1000, limit, null, 14);
				guiAmountSelector.setTooltip(tooltip);

				var guiRowItem = new GuiRowItem(2);
				guiRowItem.addChild(guiLabelItem);
				guiRowItem.addChild(guiAmountSelector);

				if (resourceType.hasFlag(64))
				{
					sectionManufacturedResources.addChild(guiRowItem);
				}
				else
				{
					sectionRawResources.addChild(guiRowItem);
				}
			}

			// Bots
			var sectionBots = new GuiSectionItem(StringList.get("manufacture_limits_bots"));

			foreach (Specialization specialization in SpecializationList.getBotSpecializations())
			{
				var tooltip = StringList.get("tooltip_manufacture_limit", specialization.getNamePlural());
				var limit = Singleton<ManufactureLimits>.getInstance().getBotLimit(specialization);

				var guiLabelItem = new GuiLabelItem(specialization.getNamePlural(), specialization.getIcon(), tooltip, 0, FontSize.Normal);

				var guiAmountSelector = new GuiAmountSelectorImproved(0, 1000, limit, null, 14);
				guiAmountSelector.setTooltip(tooltip);

				GuiRowItem guiRowItem = new GuiRowItem(2);
				guiRowItem.addChild(guiLabelItem);
				guiRowItem.addChild(guiAmountSelector);

				sectionBots.addChild(guiRowItem);
			}

			mRootItem.addChild(sectionManufacturedResources);
			mRootItem.addChild(sectionRawResources);
			mRootItem.addChild(sectionBots);
		}

		public override float getWidth()
		{
			return (float)Screen.height * 0.85f;
		}
	}
}
