using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;
using UnityEngine;
using HarmonyLib;
using System.Xml;
using System.IO;

namespace CameraOverhaul
{
    public class CameraOverhaul : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new CameraOverhaul(), modEntry, "CameraOverhaul");

        // These are not const so other mods can change them if they want
        public static float MIN_HEIGHT = 12f;
        public static float MAX_HEIGHT = 120f;

        public static float mVerticalRotationAcceleration = 0f;
        public static float mPreviousMouseY = 0f;

        public static float mAlternateRotationAcceleration = 0f;

        public static float TerrainTotalSize => CoreUtils.GetMember<float>("TotalSize", typeof(TerrainGenerator));
        public static Plane mGroundPlane = new Plane(Vector3.up, new Vector3(TerrainTotalSize, 0f, TerrainTotalSize) * 0.5f);

        public static int mModulesize = 0;
        public static bool mIsPlacingModule = false;

        public static bool ScreenEdgeScrollingEnabled = true;

        public override void OnInitialized(ModEntry modEntry)
        {
            var path = modEntry.Path + "CameraOverhaul.config";

            try
            {
                if (File.Exists(path))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(path);

                    if (bool.TryParse(xmlDoc["configuration"]["screenedgescrollingenabled"].InnerText, out bool screenEdgeScrolling))
                    {
                        ScreenEdgeScrollingEnabled = screenEdgeScrolling;
                    }
                }
            }
            catch(Exception)
            {
                // Failed to load/read the config file
            }
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing needed here
        }
    }
}
