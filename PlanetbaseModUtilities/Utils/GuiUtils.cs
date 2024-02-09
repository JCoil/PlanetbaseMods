using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseModUtilities.Utils
{
    public static class GuiUtils
    {
        public static void SetText(this GuiButtonItem button, string text)
        {
            CoreUtils.SetMember("mText", button, text);
        }
    }
}
