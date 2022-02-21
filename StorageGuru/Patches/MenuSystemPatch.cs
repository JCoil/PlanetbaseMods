using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace StorageGuru
{
    [HarmonyPatch(typeof(GuiMenuSystem), nameof(GuiMenuSystem.setActionMenu))]
    public static class MenuSystemSetActionMenuPatch
	{
		public static void Postfix(ref GuiMenuSystem __instance)
		{
			if (Selection.getSelectedConstruction() is Module module && module.isBuilt())
			{
				__instance.GetMenu("mMenuAction").getItems().Insert(1, new GuiStorageMenuItem(module));
			}
		}
    }

    /// <summary>
    /// GuiMenuItem subclass so we can easily identify in a menu's items
    /// </summary>
    public class GuiStorageMenuItem : GuiMenuItem
    {
        public GuiStorageMenuItem(Module module) : base(
                  TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStorage>().getIcon(),
                  StringList.get("tooltip_manage_storage"),
                  MenuController.OnStorageMenuItemPressed, module, FlagMenuSwitch)
        {

        }
    }
}
