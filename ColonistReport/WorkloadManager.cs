using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonistReport
{
    public class WorkloadManager : Singleton<WorkloadManager>
    {
        public static List<ColonistsWorkload> AllWorkloads { get; private set; } 

        private float mTime;
        private float mRefreshPeriod = -1f;

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
        }

        public void Update(float timeStep)
        {
            if(mRefreshPeriod < 0)
            {
                mRefreshPeriod = StatsCollector.getInstance().mRefreshPeriod / StatsCollector.RefreshesPerDay;
            }

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
