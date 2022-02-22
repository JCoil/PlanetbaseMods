using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseModUtilities
{
    public static class StringUtils
    {
        /// <summary>
        /// Add a named string into the global dictionary. Accessible by StringList.get()
        /// </summary>
        public static void RegisterString(string key, string value)
        {
            StringList.get("dummy"); // Dummy to ensure StringList.mStrings is initialised
            var mStrings = CoreUtils.GetMember<Dictionary<string, string>>("mStrings", typeof(StringList));
            mStrings.Add(key, value);
        }

        public static void SetHelpText(this HelpItem helpItem, string text) 
        {
            CoreUtils.SetMember("mText", helpItem, text);
        }
    }
}
