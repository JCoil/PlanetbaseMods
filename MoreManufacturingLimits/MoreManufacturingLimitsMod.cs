using ModWrapper;
using Planetbase;
using Redirection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreManufacturingLimits
{
    public class MoreManufacturingLimitsMod : ModBase
    {
        public override void Init()
        {
            RegisterStrings();

            Redirector.PerformRedirections();

            TypeList<ResourceType, ResourceTypeList>.find<Metal>().mFlags |= 64;

            Debug.Log("[MOD] MoreManufacturingLimits activated");
        } 

        private static void RegisterStrings()
        {
            StringList.mStrings.Add("manufacture_limits_manufactured", "Manufactured");
            StringList.mStrings.Add("manufacture_limits_raw", "Resources");
            StringList.mStrings.Add("manufacture_limits_bots", "Bots");
        }

        public override void Update(float timeStep)
        {

        }

        public override void OnGameStart()
        {
            if (MenuSystem != null && MenuSystem.mMenuBaseManagement is GuiMenu menuBaseManagement)
            {
                foreach(var menuItem in menuBaseManagement.mItems)
                {
                    if(menuItem.getRequiredModuleType() == TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>())
                    {
                        menuItem.mCallback = new GuiDefinitions.Callback(Game.toggleWindow<GuiManufactureLimitsWindowImproved>);
                    }
                }
            }
        }
    }
}
