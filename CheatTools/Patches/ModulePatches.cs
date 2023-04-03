using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheatTools.Patches
{
    [HarmonyPatch(typeof(Module), "canPlaceModule")]
    public class ModulePatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("canPlaceModule")]
        public static void Postfix1(ref bool __result)
        {
            if (CheatTools.ModuleAnarchyOn)
            {
                __result = true;
            }
        }
    }
}
