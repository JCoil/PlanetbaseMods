using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonistReport
{
    public static class WorkloadManager
    {
        public static List<ColonistsWorkload> AllWorkloads { get; private set; }

        public static void Init()
        {
            AllWorkloads = new List<ColonistsWorkload>();

            foreach (var specialization in TypeList<Specialization, SpecializationList>.get())
            {
                AllWorkloads.Add(new ColonistsWorkload(specialization));
            }
        }

        public static void Update()
        {
            foreach(var workload in AllWorkloads)
            {
                workload.Calculate();
            }
        }
    }
}
