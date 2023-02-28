using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;

namespace ColonistReports.Patches
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
                var callback = new GuiDefinitions.Callback((object parameter) => gameStateGame.toggleWindow<GuiColonistReportsWindow>());
                var reportsMenuItem = new GuiMenuItem(ResourceList.StaticIcons.Male, StringList.get("reports"), callback, 2);
                menu.AddItemBeforeBackItem(reportsMenuItem);
            }
        }
    }
}
