using System;
using UnityEngine;
using Planetbase;
using PlanetbaseModUtilities;
using HarmonyLib;

namespace ImprovedManufacturingLimits
{
	public class GuiManufactureLimitsWindowImproved : GuiWindow
	{
		public GuiManufactureLimitsWindowImproved() : 
			base(new GuiLabelItem(StringList.get("manufacture_limits"), TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>().getIcon(), null, 0, FontSize.Normal), null, null)
		{
			CoreUtils.SetMember("mHelpId", this, "manufacture_limits");

			var isFactoryBuilt = Module.isModuleTypeBuilt(TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>());

			var sectionRawResources = new GuiSectionItem(StringList.get("manufacture_limits_raw"));
			var sectionManufacturedResources = new GuiSectionItem(StringList.get("manufacture_limits_manufactured"));
			var sectionBots = new GuiSectionItem(StringList.get("manufacture_limits_bots"));

			// Resources

			GuiRowItem prevRawRow = null;
			GuiRowItem prevManufactureRow = null;

			foreach (var element in ManufactureLimitsHelper.ResourceLimits)
			{
				var resourceType = element.Key;
				var limit = element.Value;

				var tooltip = StringList.get("tooltip_manufacture_limit", element.Key.getNamePlural());

				// Alternate between adding to first column in new row and adding to second column in previous row
				if (resourceType.hasFlag(ImprovedManufacturingLimitsMod.FlagRawResource))
				{
					var guiAmountSelector = new GuiAmountSelectorImproved(0, ImprovedManufacturingLimitsMod.Settings.RawMaxValue, limit, null, 14, resourceType.getIcon(), tooltip);

					if (prevRawRow == null)
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
				else if (resourceType.hasFlag(ResourceType.FlagManufactured))
				{
					var guiAmountSelector = new GuiAmountSelectorImproved(0, ImprovedManufacturingLimitsMod.Settings.ManufacturedMaxValue, limit, null, 14, resourceType.getIcon(), tooltip);

					if (!isFactoryBuilt)
					{
						guiAmountSelector.setEnabled(false);
						guiAmountSelector.setCurrent(int.MaxValue); // Set to infinity
						guiAmountSelector.setTooltip(StringList.get("tooltip_requires", TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>().getName()));
					}

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

				var guiAmountSelector = new GuiAmountSelectorImproved(0, ImprovedManufacturingLimitsMod.Settings.BotsMaxValue, limit, null, 14, null, tooltip);

				GuiRowItem guiRowItem = new GuiRowItem(2);
				guiRowItem.addChild(guiLabelItem);
				guiRowItem.addChild(guiAmountSelector);

				sectionBots.addChild(guiRowItem);
			}

			var rootItem = getRootItem();

			rootItem.addChild(sectionRawResources);
			rootItem.addChild(sectionManufacturedResources);
			rootItem.addChild(sectionBots);
			Debug.Log("END");
		}

		public override float getWidth()
		{
			return (float)Screen.height * 0.5f;
		}
	}
}
