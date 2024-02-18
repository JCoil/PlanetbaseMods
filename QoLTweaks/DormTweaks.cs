using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;
using Planetbase;

namespace QoLTweaks
{
    public static class DormTweaks
    {
        public static void DecreaseBunkWallSeparation()
        {
            if (QoLTweaks.Settings.DecreaseBunkWallSeparation)
            {
                // Original value 1f makes it very difficult to fit in the maximum number
                // I haven't noticed any side effects of reducing this, but it is possible they may occur
                CoreUtils.SetMember("mWallSeparation", ComponentTypeList.find<Bunk>(), 1f);
            }
        }
    }
}
