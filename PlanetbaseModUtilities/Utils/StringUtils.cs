using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseModUtilities
{
    public static class StringUtils
    {
        public static Dictionary<string, string> GlobalStrings => CoreUtils.GetMember<Dictionary<string, string>>("mStrings", typeof(StringList));

        public static void SetHelpText(this HelpItem helpItem, string text) 
        {
            CoreUtils.SetMember("mText", helpItem, text);
        }
    }
}
