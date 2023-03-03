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
            CoreUtils.SetMember("mWallSeparation", ComponentTypeList.find<Bunk>(), 1f);
        }
    }
}
