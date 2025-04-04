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
using UnityModManagerNet;
using System.Reflection;

namespace CameraOverhaul
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Enable edge scrolling?(changing camera position if the cursor is on the edge of the screen) Enabled by default.")] public bool ScreenEdgeScrollingEnabled = true;
        [Draw("Minimum camera zoom (values lower than 12 may result in graphical glitches")] public float MinHeight = 12f;
        [Draw("Maximum camera zoom (values lower than 120 may result in graphical glitches")] public float MaxHeight = 120f;
        [Draw("Zoom speed multiplier. 1 by default.")] public float ZoomSpeedMultiplier = 1f;
        [Draw("Camera moves forward more as height decreases. Enabled by default.")] public bool ZoomZMotionEasingEnabled = true;
        [Draw("Camera zooms faster when it's higher. Enabled by default.")] public bool ZoomSpeedEasingEnabled = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class CameraOverhaul : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new CameraOverhaul(), modEntry, "CameraOverhaul");
            Harmony harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public static float mVerticalRotationAcceleration = 0f;
        public static float mPreviousMouseY = 0f;

        public static float mAlternateRotationAcceleration = 0f;

        public static float TerrainTotalSize => CoreUtils.GetMember<float>("TotalSize", typeof(TerrainGenerator));
        public static Plane mGroundPlane = new Plane(Vector3.up, new Vector3(TerrainTotalSize, 0f, TerrainTotalSize) * 0.5f);

        public static int mModulesize = 0;
        public static bool mIsPlacingModule = false;

        public override void OnInitialized(ModEntry modEntry)
        {
            //Nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing needed here
        }
    }
}
