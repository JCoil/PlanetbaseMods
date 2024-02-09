using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QoLTweaks.Patches
{
    //[HarmonyPatch(typeof(GuiMenuSystem), nameof(GuiMenuSystem.setEditMenu))]
    //public static class MenuSystemSetActionMenuPatch
    //{
    //    public static void Postfix(ref GuiMenu ___mMenuEdit, object[] __args)
    //    {
    //        if (__args != null && __args.Length > 0 && __args[0] is Module activeModule && activeModule.getModuleType() is ModuleTypeDorm)
    //        {
    //            var reportsMenuItem = new GuiMenuItem(ResourceList.StaticIcons.Male, StringList.get("fill_with_bunks"), (object parameter) => FillWithBunks(parameter), 2);
    //            ___mMenuEdit.AddItemBeforeBackItem(reportsMenuItem);
    //        }
    //    }

    //    private static void FillWithBunks(object parameter)
    //    {
    //        // Seems like this will need some clever geometry and raycasting
    //    }
    //}
}
