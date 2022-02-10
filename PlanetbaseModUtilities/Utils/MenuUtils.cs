using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseModUtilities
{   
    public static class MenuUtils
    {
        public static GuiMenu GetMenu(this GuiMenuSystem menuSystem, string menuName)
        {
            return CoreUtils.GetMember<GuiMenuSystem, GuiMenu>(menuName, menuSystem);
        }

        public static void SetMenu(this GuiMenuSystem menuSystem, string menuName, GuiMenu menu)
        {
            CoreUtils.SetMember<GuiMenuSystem, GuiMenu>(menuName, menuSystem, menu);
        }

        public static GuiDefinitions.Callback GetCallback(this GuiMenuItem menuItem)
        {
            return CoreUtils.GetMember<GuiMenuItem, GuiDefinitions.Callback>("mCallback", menuItem);
        }

        public static void SetCallback(this GuiMenuItem menuItem, GuiDefinitions.Callback callback)
        {
            CoreUtils.SetMember<GuiMenuItem, GuiDefinitions.Callback>("mCallback", menuItem, callback);
        }
    }
}
