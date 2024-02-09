using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CheatTools
{
    public static class CheatToolsStrings
    {
        public const string cheat_menu_title = "Cheat Tools";
        public const string cheat_menu_tooltip = "Cheat Tools";

        public const string force_structures = "Force Structures";
        public const string force_components = "Force Components";
        public const string unlock_tech = "Unlock all Tech";
        public const string clear_components = "Clear Components";

        public const string module_anarchy = "Module Anarchy";

        public static string GetModuleAnarchyText => module_anarchy + " - " + (CheatTools.ModuleAnarchyOn ? "On" : "Off");
    }
}
