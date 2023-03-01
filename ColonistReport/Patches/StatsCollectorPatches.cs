using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColonistReports.Patches
{
    [HarmonyPatch(typeof(StatsCollector))]
    public class StatsCollectorPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor)]
        public static void PostfixConstructor(ref StatsCollector __instance)
        {
            foreach (var specialization in TypeList<Specialization, SpecializationList>.get())
            {
                if (specialization is Worker)
                {
                    var workload = new SpecializationWorkload(specialization);
                    ColonistReportsMod.SpecializationWorkloads.Add(workload);
                    CoreUtils.InvokeMethod("addStat", __instance, new Type[] { typeof(string) }, new string[] { workload.StatName });
                    //CoreUtils.GetMember<StatsCollector, Dictionary<string, CountHistory>>("mData", __instance).Add(workload.StatName, new CountHistory(workload.StatName, 256));
                }
            }

            //var mRefreshperiod = CoreUtils.GetMember<StatsCollector, float>("mRefreshPeriod", __instance);
            //CoreUtils.SetMember("mRefreshPeriod", __instance, mRefreshperiod / 50f);
        }

        [HarmonyPostfix]
        [HarmonyPatch("refresh")]
        public static void PostfixRefresh(ref StatsCollector __instance)
        {
            // Refresh occurs once on game load, then once every ~100 seconds

            foreach (var workload in ColonistReportsMod.SpecializationWorkloads)
            {
                if(workload.IsSaturated)
                {
                    __instance.getData()[workload.StatName].add((int)(workload.PollAverageWorkload() *100f));
                }
            }
        }
    }
}
