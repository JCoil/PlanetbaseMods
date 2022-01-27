using System;
using UnityEngine;
using Planetbase;

namespace ImprovedManufacturingLimits
{
	public class GuiManufactureLimitsWindowImproved : GuiWindow
	{
		public GuiManufactureLimitsWindowImproved() : base(new GuiLabelItem(StringList.get("manufacture_limits"), TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>().getIcon(), null, 0, FontSize.Normal), null, null)
		{
			this.mHelpId = "manufacture_limits";

			var sectionRawResources = new GuiSectionItem(StringList.get("manufacture_limits_raw"));
			var sectionManufacturedResources = new GuiSectionItem(StringList.get("manufacture_limits_manufactured"));
			var sectionBots = new GuiSectionItem(StringList.get("manufacture_limits_bots"));

			// Resources

			GuiRowItem prevRawRow = null;
			GuiRowItem prevManufactureRow = null;

			foreach (var element in Singleton<ManufactureLimits>.getInstance().mResourceLimits)
			{
				var resourceType = element.Key;
				var limit = element.Value;

				var tooltip = StringList.get("tooltip_manufacture_limit", element.Key.getNamePlural());

				var guiAmountSelector = new GuiAmountSelectorImproved(0, ImprovedManufacturingLimitsMod.RawMaxValue, limit, null, 14, resourceType.getIcon());
				guiAmountSelector.setTooltip(tooltip);

				// Alternate between adding to first column in new row and adding to second column in previous row
				if (resourceType.hasFlag(ImprovedManufacturingLimitsMod.FlagRawResource))
				{
					guiAmountSelector.mMax = ImprovedManufacturingLimitsMod.RawMaxValue;

					if(prevRawRow == null)
					{
						var newRowItem = new GuiRowItem(2);
						newRowItem.addChild(guiAmountSelector);
						sectionRawResources.addChild(newRowItem);

						prevRawRow = newRowItem;
					}
					else
					{
						prevRawRow.addChild(guiAmountSelector);
						prevRawRow = null;
					}
				}
				else if(resourceType.hasFlag(ResourceType.FlagManufactured))
				{
					guiAmountSelector.mMax = ImprovedManufacturingLimitsMod.ManufacturedMaxValue;

					if (prevManufactureRow == null)
					{
						var newRowItem = new GuiRowItem(2); 
						newRowItem.addChild(guiAmountSelector);
						sectionManufacturedResources.addChild(newRowItem);

						prevManufactureRow = newRowItem;
					}
					else
					{ 
						prevManufactureRow.addChild(guiAmountSelector);
						prevManufactureRow = null;
					}
				}
			}

			// Bots

			foreach (Specialization specialization in SpecializationList.getBotSpecializations())
			{
				var tooltip = StringList.get("tooltip_manufacture_limit", specialization.getNamePlural());
				var limit = Singleton<ManufactureLimits>.getInstance().getBotLimit(specialization);

				var guiLabelItem = new GuiLabelItem(specialization.getNamePlural(), specialization.getIcon(), tooltip, 0, FontSize.Normal);

				var guiAmountSelector = new GuiAmountSelectorImproved(0, ImprovedManufacturingLimitsMod.BotsMaxValue, limit, null, 14, null);
				guiAmountSelector.setTooltip(tooltip);

				GuiRowItem guiRowItem = new GuiRowItem(2);
				guiRowItem.addChild(guiLabelItem);
				guiRowItem.addChild(guiAmountSelector);

				sectionBots.addChild(guiRowItem);
			}

			mRootItem.addChild(sectionRawResources);
			mRootItem.addChild(sectionManufacturedResources);
			mRootItem.addChild(sectionBots);
		}

		public override float getWidth()
		{
			return (float)Screen.height * 0.5f;
		}
	}
}
