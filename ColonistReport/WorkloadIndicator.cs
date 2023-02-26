using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ColonistReports
{
    public class WorkloadIndicator : Indicator
    {
        public WorkloadIndicator(string name, Texture2D icon) : base(name, icon, IndicatorType.Progress)
        {

        }
	}
}
