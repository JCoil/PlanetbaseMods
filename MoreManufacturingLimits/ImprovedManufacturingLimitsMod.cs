using ModWrapper;
using Planetbase;
using Redirection;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using System.Xml;

namespace ImprovedManufacturingLimits
{
    public class ImprovedManufacturingLimitsMod : ModBase, IMod
    {
        readonly string ConfigFilePath = "/Mods/ImprovedManufacturingLimitsData/config.txt";

        public static int RawMaxValue = 9999;
        public static int ManufacturedMaxValue = 999;
        public static int BotsMaxValue = 99;

        public const int FlagRawResource = 256;
        private List<ResourceType> NewResourceLimits;

        public override void Init()
        {
            LoadConfig();
            RegisterStrings();

            AddFlagsToNewResources();

            Redirector.PerformRedirections();


            Debug.Log("[MOD] MoreManufacturingLimits activated");
        }

        private void LoadConfig()
        {
            //try
            //{
            //    // Initial config file generation
            //    //var xmlNode = Serialization.beginSave(Util.getFilesFolder() + ConfigFilePath, "config");
            //    //Serialization.serializeInt(xmlNode, "limitmaxvalue", 999);
            //    //Serialization.endSave();

            //    var rootNode = Serialization.beginLoad(Util.getFilesFolder() + ConfigFilePath);
            //    RawMaxValue = Serialization.deserializeInt(rootNode["rawmaxvalue"]);
            //    ManufacturedMaxValue = Serialization.deserializeInt(rootNode["manufacturedmaxvalue"]);
            //    BotsMaxValue = Serialization.deserializeInt(rootNode["botsmaxvalue"]);
            //}
            //catch (Exception ex)
            //{
            //    Debug.Log("Failed to deserialize ImprovedManufacturingLimits config: " + ex.Message);
            //}
        }

        private void AddFlagsToNewResources()
        {
            NewResourceLimits = new List<ResourceType>()
            {
                TypeList<ResourceType, ResourceTypeList>.find<Metal>(),
                TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(),
                TypeList<ResourceType, ResourceTypeList>.find<MedicinalPlants>(),
                TypeList<ResourceType, ResourceTypeList>.find<Vegetables>(),
                TypeList<ResourceType, ResourceTypeList>.find<Vitromeat>(),
                TypeList<ResourceType, ResourceTypeList>.find<Starch>(),
                TypeList<ResourceType, ResourceTypeList>.find<Ore>(),
            };

            foreach (var resourceType in NewResourceLimits)
            {
                resourceType.mFlags |= ResourceType.FlagManufactured; // Add manufactured flag so the resource is picked up by ManufactureLimits
                resourceType.mFlags |= FlagRawResource; // Add custom flag to differentiate between raw and manufactured
            }
        }

        private static void RegisterStrings()
        {
            Singleton<HelpManager>.getInstance().findItem("manufacture_limits").mText = HelpText;

            StringList.mStrings.Add("manufacture_limits_raw", "Resources");
            StringList.mStrings.Add("manufacture_limits_manufactured", "Manufactured");
            StringList.mStrings.Add("manufacture_limits_bots", "Bots");
        }

        public override void Update(float timeStep)
        {

        }

        public override void OnGameStart()
        {
            // Replace default limits window with our improved one
            if (MenuSystem != null && MenuSystem.mMenuBaseManagement is GuiMenu menuBaseManagement)
            {
                foreach (var menuItem in menuBaseManagement.mItems)
                {
                    if (menuItem.getRequiredModuleType() == TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>())
                    {
                        menuItem.mCallback = new GuiDefinitions.Callback(Game.toggleWindow<GuiManufactureLimitsWindowImproved>);
                    }
                }
            }
        }

        const string HelpText = @"The <b><color=""#44FF73"">Manufacturing Limits</color></b> panel allows you to specify the maximum number of units for the various raw resources, manufactured goods and bots. After you hit the limit, no more of that item will be produced.

Producers that require maintenance, such as <b><color=""#44FF73"">Vegetable Pads</color></b> and <b><color=""#44FF73"">Tissue Synthesizers</color></b>, will be ignored. Colonists will not work in <b><color=""#44FF73"">Mines</color></b> and components that convert raw resources into other materials, such as <b><color=""#44FF73"">Metal Processors</color></b> and <b><color=""#44FF73"">Bioplastic Processors</color></b>, will not be kept stocked.

Most of these goods require <b><color=""#44FF73"">Bioplastic</color></b> and <b><color=""#44FF73"">Metal</color></b> to produce, so it is sometimes a good idea to limit the production to avoid exhausting your basic <b><color=""#44FF73"">resources</color></b>.";
    }
}
