using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;
using UnityModManagerNet;

namespace ImprovedManufacturingLimits
{
    public class ImprovedManufacturingLimitsSettings : ModSettings, IDrawable
    {
        [Draw("Raw Resources maximum value")] public int RawMaxValue = 10000;
        [Draw("Manufactured Resources maximum value")] public int ManufacturedMaxValue = 1000;
        [Draw("Bots maximum value")] public int BotsMaxValue = 100;

        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }

    public class ImprovedManufacturingLimitsMod : ModBase
    {
        public static ImprovedManufacturingLimitsSettings Settings;

        public static new void Init(ModEntry modEntry) => InitializeMod(new ImprovedManufacturingLimitsMod(), modEntry, "ImprovedManufacturingLimits");        

        public const int FlagRawResource = 256;
        private List<ResourceType> NewResourceLimits;

        public override void OnInitialized(ModEntry modEntry)
        {
            Settings = ModSettings.Load<ImprovedManufacturingLimitsSettings>(modEntry);

            modEntry.OnGUI = (ModEntry entry) => Settings.Draw(modEntry);
            modEntry.OnSaveGUI = (ModEntry entry) => Settings.Save(modEntry);

            RegisterStrings();

            AddFlagsToNewResources();

            Debug.Log("[MOD] MoreManufacturingLimits activated");
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
                var resourceTypeFlags = CoreUtils.GetMember<ResourceType, int>("mFlags", resourceType);
                resourceTypeFlags |= ResourceType.FlagManufactured; // Add manufactured flag so the resource is picked up by ManufactureLimits
                resourceTypeFlags |= FlagRawResource; // Add custom flag to differentiate between raw and manufactured

                CoreUtils.SetMember("mFlags", resourceType, resourceTypeFlags);
            }
        }

        private static void RegisterStrings()
        {
            CoreUtils.SetMember("mText", Singleton<HelpManager>.getInstance().findItem("manufacture_limits"), HelpText);

            StringUtils.RegisterString("manufacture_limits_raw", "Resources");
            StringUtils.RegisterString("manufacture_limits_manufactured", "Manufactured");
            StringUtils.RegisterString("manufacture_limits_bots", "Bots");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing required here
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            // Nothing required here
        }

        const string HelpText = @"The <b><color=""#44FF73"">Manufacturing Limits</color></b> panel allows you to specify the maximum number of units for the various raw resources, manufactured goods and bots. After you hit the limit, no more of that item will be produced.

Producers that require maintenance, such as <b><color=""#44FF73"">Vegetable Pads</color></b> and <b><color=""#44FF73"">Tissue Synthesizers</color></b>, will be ignored. Colonists will not work in <b><color=""#44FF73"">Mines</color></b> and components that convert raw resources into other materials, such as <b><color=""#44FF73"">Metal Processors</color></b> and <b><color=""#44FF73"">Bioplastic Processors</color></b>, will not be kept stocked.

Most of these goods require <b><color=""#44FF73"">Bioplastic</color></b> and <b><color=""#44FF73"">Metal</color></b> to produce, so it is sometimes a good idea to limit the production to avoid exhausting your basic <b><color=""#44FF73"">resources</color></b>.";
    }
}
