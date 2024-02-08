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
			if (Selection.getSelectedConstruction() is Module module && module.isBuilt() && module.GetCategory() == Module.Category.Storage)
			{
				__instance.GetMenu("mMenuAction").getItems().Insert(1, new GuiStorageMenuItem(module));
			}
		}
    }

    /// <summary>
    /// We still need to clear the edit menu then set its value and copy to current menu.
    /// However, these happen at the start and end of the original code, so we'd have to use a transpiler to reuse them.
    /// Since it's just a couple of lines we'll just use prefix and copy them here with our own code in the middle.
    /// </summary>
    [HarmonyPatch(typeof(GuiMenuSystem), nameof(GuiMenuSystem.setEditMenu))]
    public static class MenuSystemSetEditMenuPatch
    {
        public static bool Prefix(ref GuiMenuSystem __instance, Module activeModule)
        {
            if (activeModule.isBuilt() && activeModule.GetCategory() == Module.Category.Storage)
            {
                __instance.GetMenu("mMenuEdit").clear();

                if (Selection.getSelected() == null)
                {
                    Selection.select(activeModule);
                }

                var storageMenu = new GuiStorageMenu(activeModule);
                __instance.SetMenu("mMenuEdit", storageMenu);
                __instance.SetMenu("mCurrentMenu", storageMenu);

                return false;
            }

            return true; // If not storage, we want to run original method
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
                  OnStorageMenuItemPressed, module, FlagMenuSwitch)
        {

        }

        /// <summary>
        /// While we're viewing the menu to configure a storage module, we'll pretend we're actually editing it for the most part, using Mode.EditingModule.
        /// That way, all the original codes handling of Esc, deselection etc. will work without us having to reimplement.
        /// </summary>
        /// <param name="parameter"></param>
        public static void OnStorageMenuItemPressed(object parameter)
        {
            var gameStateGame = (GameStateGame)GameManager.getInstance().getGameState();
            CoreUtils.InvokeMethod("startEditingModule", gameStateGame);
        }
    }
}
