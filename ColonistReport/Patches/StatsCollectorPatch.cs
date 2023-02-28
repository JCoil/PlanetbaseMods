using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;

namespace ColonistReports.Patches
{
    ///// <summary>
    ///// Patch GuiMenuSystem init to replace manufacture limits callback with our improved window
    ///// </summary>
    //[HarmonyPatch(typeof(StatsCollector), MethodType.Constructor)]
    //public class StatsCollectorPatch
    //{
    //    public static void Postfix(ref StatsCollector __instance)
    //    {
    //        CoreUtils.InvokeMethod("addStat", __instance, "WorkerWorkload");
    //        CoreUtils.InvokeMethod("addStat", __instance, "MedicWorkload");
    //        CoreUtils.InvokeMethod("addStat", __instance, "BiologistWorkload");
    //    }
    //}
}
