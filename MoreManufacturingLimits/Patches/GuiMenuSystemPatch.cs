using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ImprovedManufacturingLimits
{
	/// <summary>
	/// Patch GuiMenuSystem init to replace manufacture limits callback with our improved window
	/// </summary>
	[HarmonyPatch(typeof(GuiMenuSystem), nameof(GuiMenuSystem.init))]
	public class GuiMenuSystemPatch
	{
		public static void Postfix(ref GuiMenuSystem __instance, GameStateGame gameStateGame)
		{
			if (__instance.GetMenu("mMenuBaseManagement") is GuiMenu menu)
            {
				Debug.Log("BASE MANAGEMENT");
				Debug.Log(menu.getItemCount());

				// Unfortunately the menu items are all generic, so the best way to identify them is by matching required ModuleType
				var moduleType = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>();
				if (menu.getItems().FirstOrDefault(x=> x.getRequiredModuleType() == moduleType) is GuiMenuItem manufacturLimitsMenuItem)
				{
					Debug.Log("MAN LIMITS");
					manufacturLimitsMenuItem.SetCallback(new GuiDefinitions.Callback(gameStateGame.toggleWindow<GuiManufactureLimitsWindowImproved>));
                }
            }
		}
	}
}
