using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;
using UnityEngine;
using System.Reflection;

namespace ColonistReports
{
    public class WorkloadManager : Singleton<WorkloadManager>
    {
        const float RefreshesPerDay = 2f;
        public static List<ColonistsWorkload> AllWorkloads { get; private set; } 

        private float mTime;
        private float mRefreshPeriod = 1f;

        public WorkloadManager()
        {
            AllWorkloads = new List<ColonistsWorkload>();

            foreach (var specialization in TypeList<Specialization, SpecializationList>.get())
            {
                if (!(specialization is Visitor || specialization is Intruder))
                {
                    AllWorkloads.Add(new ColonistsWorkload(specialization));
                }
            }

            mTime = mRefreshPeriod;
        }

        public void Update(float timeStep)
        {
            foreach (var workload in AllWorkloads)
            {
                workload.Update();
            }

            mTime += timeStep;
            if (mTime > mRefreshPeriod)
            {
                foreach (var workload in AllWorkloads)
                {
                    workload.Refresh();
                }
                mTime = 0f;
            }
        }
    }
}
