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

        public static float mVerticalRotationAcceleration = 0f;
        public static float mPreviousMouseY = 0f;

        public static float mAlternateRotationAcceleration = 0f;

        public static float TerrainTotalSize => CoreUtils.GetMember<float>("TotalSize", typeof(TerrainGenerator));
        public static Plane mGroundPlane = new Plane(Vector3.up, new Vector3(TerrainTotalSize, 0f, TerrainTotalSize) * 0.5f);

        public static int mModulesize = 0;
        public static bool mIsPlacingModule = false;

        // User configurable settings
        public static bool ScreenEdgeScrollingEnabled = true;
        public static float MinHeight = 12f;
        public static float MaxHeight = 120f;
        public static float ZoomSpeedMultiplier = 1f;
        public static bool ZoomZMotionEasingEnabled = true;
        public static bool ZoomSpeedEasingEnabled = true;

        public override void OnInitialized(ModEntry modEntry)
        {
            LoadUserSettings(modEntry.Path + "CameraOverhaul.config");
        }

        private void LoadUserSettings(string path)
        {
            if(!File.Exists(path))
            {
                return;
            }

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                var rootNode = xmlDoc["configuration"];

                if (bool.TryParse(rootNode["screenedgescrollingenabled"].InnerText, out bool screenEdgeScrollingEnabled))
                {
                    ScreenEdgeScrollingEnabled = screenEdgeScrollingEnabled;
                }

                if (float.TryParse(rootNode["minheight"].InnerText, out float minHeight) && minHeight > 0)
                {
                    MinHeight = minHeight;
                }

                if (float.TryParse(rootNode["maxheight"].InnerText, out float maxHeight) && maxHeight > MinHeight)
                {
                    MaxHeight = maxHeight;
                }

                if (float.TryParse(rootNode["zoomspeedmultiplier"].InnerText, out float speedMultiplier) && speedMultiplier > 0)
                {
                    ZoomSpeedMultiplier = speedMultiplier;
                }

                if (bool.TryParse(rootNode["zoomzmotioneasingenabled"].InnerText, out bool zMotionEasing))
                {
                    ZoomZMotionEasingEnabled = zMotionEasing;
                }

                if (bool.TryParse(rootNode["zoomspeedeasingenabled"].InnerText, out bool speedEasing))
                {
                    ZoomSpeedEasingEnabled = speedEasing;
                }
            }
            catch (Exception)
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
