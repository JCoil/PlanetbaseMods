using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseModUtilities
{
    public static class StringUtils
    {
        public static Dictionary<string, string> GetGlobalStrings() => CoreUtils.GetMember<Dictionary<string, string>>("mStrings", typeof(StringList));
    }
}
