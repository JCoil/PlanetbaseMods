using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using System.Reflection;
using PlanetbaseModUtilities;

namespace ColonistReports
{
    public class ColonistReportsMod : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new ColonistReportsMod(), modEntry, "ColonistReports");

        public static List<SpecializationWorkload> SpecializationWorkloads { get; private set; }

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();

            SpecializationWorkloads = new List<SpecializationWorkload>();

            Debug.Log("[MOD] Colonist Reports activated");
        }

        private void RegisterStrings()
        {
            StringUtils.RegisterString("reports", "Colonist Reports");
            StringUtils.RegisterString("reports_workload", "Colonist Workload");

            StringUtils.RegisterString("reports_worker_workload", "Worker Workload");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            foreach (var workload in SpecializationWorkloads)
            {
                workload.Update();
            }
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            //if (Singleton<StatsCollector>.mInstance is StatsCollector statsCollector)
            //{
            //    CoreUtils.InvokeMethod("addStat", statsCollector, "WorkerWorkload");
            //    CoreUtils.InvokeMethod("addStat", statsCollector, "MedicWorkload");
            //    CoreUtils.InvokeMethod("addStat", statsCollector, "BiologistWorkload");
            //}
        }
    }
}
